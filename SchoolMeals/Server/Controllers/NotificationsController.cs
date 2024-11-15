using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using SchoolMeals.Server.Interfices;
using SchoolMeals.Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolMeals.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() { return Ok("Oki"); }

        [HttpPut]
        [Route("installations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> UpdateInstallation(
            [Required] DeviceInstallation deviceInstallation)
        {
            var success = await _notificationService
                .CreateOrUpdateInstallationAsync(deviceInstallation, HttpContext.RequestAborted);

            if (!success)
                return new UnprocessableEntityResult();

            return new OkResult();
        }

        [HttpDelete()]
        [Route("installations/{installationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<ActionResult> DeleteInstallation(
            [Required][FromRoute] string installationId)
        {
            // Probably want to ensure deletion even if the connection is broken
            var success = await _notificationService
                .DeleteInstallationByIdAsync(installationId, CancellationToken.None);

            if (!success)
                return new UnprocessableEntityResult();

            return new OkResult();
        }

        [HttpPost("requests")]
        //[Route("requests")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> RequestPush([Required] NotificationRequest notificationRequest)
        {
            if ((notificationRequest.Silent && string.IsNullOrWhiteSpace(notificationRequest?.Action)) ||
                (!notificationRequest.Silent && string.IsNullOrWhiteSpace(notificationRequest?.Text)))
                return new BadRequestResult();

                var success = await _notificationService
                    .RequestNotificationAsync(notificationRequest, HttpContext.RequestAborted);

            if (!success)
                return new UnprocessableEntityResult();

            return new OkResult();
        }
    }
}
