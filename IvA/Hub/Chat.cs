using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IvA
{

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

        // Die Nachrichten des Absenders und des Empfängers sind Aktualisierungen. 
        // Falls die Nachricht vom Empfänger nicht gelesen wird, wird eine Benachrichtigung mit der Anzahl der ungelesenen Nachrichten erstellt

        public async Task MessageUpdate(string usuario)
        {
            var query = _context.Message
                    .AsQueryable()
                    .Where(m =>
                    (m.QuellID == Context.User.Identity.Name && m.ZielID == usuario) ||
                    (m.QuellID == usuario && m.ZielID == Context.User.Identity.Name)
                    ).OrderByDescending(x => x.MessageID).Take(25)
                    .ToList();
            await Clients.Client(Context.ConnectionId).SendAsync("MessageUpdate", query);
            var query2 = _context.Message
                    .AsQueryable()
                    .Where(m =>
                    (m.Status == false)
                    )
                    .ToList();
            await Clients.All.SendAsync("mensajesnoleidos", query2);
            await Clients.Client(Context.ConnectionId).SendAsync("mensajesnoleidos", query2);
        }

        // Funktion, die der Messagetabelle eine Nachricht mit den Daten von Absender, Empfänger, Nachricht und Datum hinzufügt
        public async Task SendMessagePrivate(string destinatario, string destinatarioNombre, string mensaje)
        {
            var DataMensaje = new Message();
            DataMensaje.QuellID = Context.User.Identity.Name;
            DataMensaje.ZielID = destinatarioNombre;
            DataMensaje.Nachricht = mensaje;
            DataMensaje.Datum = DateTime.Now;
            _context.Message.Add(DataMensaje);
            _context.SaveChanges();
            await Clients.Client(destinatario).SendAsync("ReceiveMessaggePrivate", Context.User.Identity.Name, Context.ConnectionId, mensaje);
        }

        // Wenn eine ungelesene Nachricht gelesen wird, wird der Status des Nachrichts in True geändert
        public async Task readmessage(string remitente, string destinatario)
        {
            _context.Message.Where(r => r.Status == false && r.QuellID == destinatario && r.ZielID == remitente).ToList().ForEach(x =>
           {
               x.Status = true;
           });

            _context.SaveChanges();

            var query = _context.Message
                    .AsQueryable()
                    .Where(m =>
                    (m.QuellID == destinatario && m.ZielID == remitente));
            await Clients.Client(Context.ConnectionId).SendAsync("readmessage", destinatario, query);
        }

        // Wenn sich ein Benutzer angemeldet, wird er zu Tabelle UserLogin hinzugefügt, sodass er im Online-Benutzerfenster angezeigt wird
        public override async Task OnConnectedAsync()
        {
            if (_context.UserLogin.Where(x => x.UserName == Context.User.Identity.Name).FirstOrDefault() != null)
            {
                var usuario1 = new UserLogin();
                usuario1 = _context.UserLogin.Where(x => x.UserName == Context.User.Identity.Name).FirstOrDefault();
                _context.UserLogin.Remove(usuario1);
                _context.SaveChanges();
            }
            var usuario = new UserLogin();
            usuario.UserName = Context.User.Identity.Name;
            usuario.UserID = Context.ConnectionId;
            _context.UserLogin.Add(usuario);
            _context.SaveChanges();
            var usuariosconectados = from d in _context.UserLogin select d;
            await Clients.All.SendAsync("ClientUpdate", usuariosconectados);
            var query = _context.Message
                    .AsQueryable()
                    .Where(m =>
                    (m.Status == false)
                    )
                    .ToList();
            await Clients.Client(Context.ConnectionId).SendAsync("myUser", Context.User.Identity.Name);
            await Clients.All.SendAsync("mensajesnoleidos", query);
            await Clients.Client(Context.ConnectionId).SendAsync("mensajesnoleidos", query);
            await base.OnConnectedAsync();
        }

        // Wenn sich ein Benutzer abmeldet oder das Collab-Fenster schließt, wird der Benutzer aus der Online-Benutzertabelle entfernt, 
        // danach die Liste der verbundenen Benutzer für alle anderen Benutzer aktualisiert wurde 
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var usuario = new UserLogin();
            usuario = _context.UserLogin.Where(x => x.UserName == Context.User.Identity.Name).FirstOrDefault();
            _context.UserLogin.Remove(usuario);
            _context.SaveChanges();
            var usuariosconectados = from d in _context.UserLogin select d;
            await Clients.All.SendAsync("ClientUpdate", usuariosconectados);
            var query = _context.Message
                    .AsQueryable()
                    .Where(m =>
                    (m.Status == false)
                    )
                    .ToList();
            await Clients.Client(Context.ConnectionId).SendAsync("myUser", Context.User.Identity.Name);
            await Clients.All.SendAsync("mensajesnoleidos", query);
            await Clients.Client(Context.ConnectionId).SendAsync("mensajesnoleidos", query);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
