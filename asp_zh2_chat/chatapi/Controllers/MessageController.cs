using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using chatapi.Data;
using chatapi.Models;
using chatapi.ViewModels;
using JwtAuthentication.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace chatapi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        ApplicationDbContext _context;
        UserManager<IdentityUser> _userManager;
        IHubContext<ChatHub> _hub;

        public MessageController(ApplicationDbContext ctx, UserManager<IdentityUser> users, IHubContext<ChatHub> hub)
        {
            this._context = ctx;
            this._userManager = users;
            this._hub = hub;
        }

        [HttpGet("{RoomId}")]
        public IEnumerable<Message> GetMessagesByRoomId(string RoomId)
        {
            var l = this._context.Rooms.SingleOrDefault(x => x.Uid.Equals(RoomId));

            if (l == default)
                return null;

            return l.Messages;
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] MessageViewModel value)
        {
            var myself = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var m = new Message
            {
                RoomUid = value.RoomUid,
                Uid = Guid.NewGuid().ToString(),
                MessageText = value.MessageText
            };

            if (value.Uid == null)
                value.Uid = Guid.NewGuid().ToString();

            m.Sender = await this._userManager.FindByEmailAsync(myself);

            this._context.Messages.Add(m);
            var l = this._context.Rooms.SingleOrDefault(x => x.Uid.Equals(value.RoomUid));
            l.Messages.Add(m);

            this._context.SaveChanges();

            await this._hub.Clients.All.SendAsync("NewMessage", value);
            return Ok();
        }
    }
}
