using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chatapi.Data;
using chatapi.Models;
using chatapi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using JwtAuthentication.Hubs;

namespace chatapi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RoomController : ControllerBase
    {
        ApplicationDbContext _context;
        UserManager<IdentityUser> _userManager;
        IHubContext<ChatHub> _hub;

        public RoomController(ApplicationDbContext ctx, UserManager<IdentityUser> users, IHubContext<ChatHub> hub)
        {
            this._context = ctx;
            this._userManager = users;
            this._hub = hub;
        }

        [HttpGet]
        public IEnumerable<RoomViewModel> GetAll()
        {
            return this._context.Rooms.Select(x => new RoomViewModel
            {
                RoomName = x.RoomName,
                Uid = x.Uid,
                MessageCount = x.Messages.Count
            });
        }

        [HttpGet("{uid}")]
        public RoomViewModel GetOne(string uid)
        {
            var r = this._context.Rooms.SingleOrDefault(x => x.Uid.Equals(uid));
            return new RoomViewModel
            {
                Uid = r.Uid,
                RoomName = r.RoomName,
                MessageCount = r.Messages.Count
            };
        }

        [HttpPost]
        public async Task<ActionResult> CreateNewRoom([FromBody] RoomViewModel value)
        {
            var myself = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var l = new Room
            {
                Uid = Guid.NewGuid().ToString(),
                Creator = await this._userManager.FindByEmailAsync(myself),
                RoomName = value.RoomName,
                Messages = new List<Message>()
            };

            this._context.Rooms.Add(l);
            this._context.SaveChanges();

            await _hub.Clients.All.SendAsync("NewRoom", value);
            return Ok();
        }
    }
}
