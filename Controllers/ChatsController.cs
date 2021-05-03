using backend_CA.Data;
using backend_CA.Models;
using backend_CA.Services;
using backend_CA.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_CA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ChatsController : Controller
    {
        private IChatService _chatService;
        private readonly Context _context;
        public ChatsController(Context context, IChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        //-----
        //Method to get a list of every room Id owned by the user
        //-----
        [HttpGet("getRooms")]
        public ActionResult<IEnumerable<Chat>> GetRooms()
        {
            return _context.chats.ToList().FindAll(x => x.receiverId.Equals(getUserId()) || x.owner.Equals(getUserId()));
        }

        //-----
        //Method to get a list of every users inside a room
        //-----
        [HttpGet("getUsersInRooms")]
        public ActionResult<IEnumerable<User>> GetUsersInRooms()
        {
            return _chatService.getRoomMember(getUserId());
        }

        //-----
        //Method to get a message
        //-----
        [HttpGet("getMessages")]
        public ActionResult<IEnumerable<Message>> GetChatMessages(int roomId, int limit)
        {
            try
            {
                return _chatService.getChatMessages(roomId, limit, getUserId());
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Method to send a message
        //-----
        [HttpPost("sendMessage")]
        public ActionResult<Message> SendMessage(MessageModel model)
        {
            try
            {
                _chatService.sendMessage(model.roomId, getUserId(), model.content);
                return Ok(new { message = "Sucessfully sent " + model.content + " to room " + model.roomId });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Method to create a room
        //-----
        [HttpPost("createRoom")]
        public ActionResult CreateChatRoom(CreateRoomModel model)
        {
            try
            {
                _chatService.createRoom(getUserId(), model.roomName, model.roomDescription, model.receiverId);
                return Ok(new { message = "Sucessfully created the room !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Method to delete a room
        //-----
        [HttpDelete("deleteRoom")]
        public ActionResult DeleteChatRoom(int roomId)
        {
            try
            {
                _chatService.deleteRoom(roomId, getUserId());
                return Ok(new { message = "Sucessfully deleted the room and every linked messages !" });
            }
            catch (CustomException e)
            {
                return BadRequest(new { message = e.ToString() });
            }
        }

        //-----
        //Returns the id of the currently logged account
        //-----
        private int getUserId()
        {
            return int.Parse(User.Identity.Name);
        }
    }
}
