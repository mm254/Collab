﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IvA.Data;
using IvA.Models;
using Microsoft.EntityFrameworkCore;

namespace IvA
{
    /**/
      public class Chat : Hub
    {
        private readonly ApplicationDbContext _context;
        public Chat(ApplicationDbContext context)
        {
            _context = context;
        }        
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name ?? "anonymous", message);
        }

        public async Task MessageUpdate(string usuario)
        {
            var query = _context.Message
                    .AsQueryable()
                    .Where(m =>
                    (m.QuellID == Context.User.Identity.Name && m.ZielID == usuario) ||
                    (m.QuellID == usuario && m.ZielID == Context.User.Identity.Name)
                    ).Take(25)
                    .ToList();           
            await Clients.Client(Context.ConnectionId).SendAsync("MessageUpdate", query);           
        }

        
        public async Task SendMessagePrivate(string destinatario, string destinatarioNombre, string mensaje)
        {
            var DataMensaje = new Message();
            DataMensaje.QuellID = Context.User.Identity.Name;
            DataMensaje.ZielID = destinatarioNombre;
            DataMensaje.Nachricht = mensaje;
            DataMensaje.Status = false;
            DataMensaje.Datum = DateTime.Now;
            _context.Message.Add(DataMensaje);
            _context.SaveChanges();
            await Clients.Client(destinatario).SendAsync("ReceiveMessaggePrivate", Context.User.Identity.Name,Context.ConnectionId , mensaje);
        }
 
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public override async Task OnConnectedAsync()
        {            
            var usuario = new UserLogin();
            usuario.UserName = Context.User.Identity.Name;
            usuario.UserID = Context.ConnectionId;
            _context.UserLogin.Add(usuario);
            _context.SaveChanges();
            var usuariosconectados = from d in _context.UserLogin select d;
            await Clients.All.SendAsync("ClientUpdate", usuariosconectados);
            await Clients.Client(Context.ConnectionId).SendAsync("myUser", Context.User.Identity.Name);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var usuario = new UserLogin();
                usuario = _context.UserLogin.Where(x => x.UserName == Context.User.Identity.Name).FirstOrDefault();
                _context.UserLogin.Remove(usuario);
                _context.SaveChanges();            
            var usuariosconectados = from d in _context.UserLogin select d;
            await Clients.All.SendAsync("ClientUpdate", usuariosconectados);
            await Clients.Client(Context.ConnectionId).SendAsync("myUser", Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }

    }
}