using Microsoft.AspNetCore.Mvc;

namespace StarterKit.Api.Controllers;

[ApiController]
[Route("v1/[controller]")] // Global prefix /v1/controllerName
public abstract class BaseController : ControllerBase
{
}