using Microsoft.AspNetCore.Mvc;
using lcpapi.Services;
using lcpapi.Models;
using QRCoder;
using System;
using System.IO;
using lcpapi.Context;

namespace lcpapi.Controllers;

[ApiController]
[Route("auth/otp")]
[Authorize]
[Produces("application/json")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otpService;
    private readonly MyDBContext _context;

    public OtpController(IOtpService otpService, MyDBContext context)
    {
        _otpService = otpService;
        _context = context;
    }

    // Create a new secret and return provisioning URI (authenticated)
    [HttpPost("setup")]
    public IActionResult Setup()
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null) return Unauthorized();

        var secret = _otpService.GenerateSecret();
        var uri = _otpService.GetProvisioningUri(user.Username ?? user.Id.ToString(), "LCPApi", secret);

        // store the secret temporarily but do not enable yet
        user.OtpSecret = secret;
        _context.Update(user);
        _context.SaveChanges();

        // generate QR PNG bytes
        var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var pngBytes = new PngByteQRCode(qrData).GetGraphic(20);
        var pngBase64 = Convert.ToBase64String(pngBytes);
        var dataUri = $"data:image/png;base64,{pngBase64}";

        return Ok(new { secret, uri, pngBase64, dataUri });
    }

    // Create a new secret and return raw PNG (authenticated)
    [HttpGet("setup/png")]
    [Produces("image/png")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult SetupPng()
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null) return Unauthorized();

        var secret = _otpService.GenerateSecret();
        var uri = _otpService.GetProvisioningUri(user.Username ?? user.Id.ToString(), "LCPApi", secret);

        // store the secret temporarily but do not enable yet
        user.OtpSecret = secret;
        _context.Update(user);
        _context.SaveChanges();

        var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var pngBytes = new PngByteQRCode(qrData).GetGraphic(20);

        return File(pngBytes, "image/png", "otp-qr.png");
    }

    // Enable OTP for the current user after validating code
    [HttpPost("enable")]
    public IActionResult Enable([FromBody] EnableOtpRequest model)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null) return Unauthorized();
        if (string.IsNullOrEmpty(user.OtpSecret)) return BadRequest(new { message = "No OTP secret found. Call setup first." });

        if (!_otpService.ValidateTotp(user.OtpSecret, model.Code))
            return BadRequest(new { message = "Invalid OTP code." });

        user.OtpEnabled = true;
        _context.Update(user);
        _context.SaveChanges();

        return Ok(new { message = "OTP enabled" });
    }

    // Disable OTP for the current user after validating code
    [HttpPost("disable")]
    public IActionResult Disable([FromBody] EnableOtpRequest model)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null) return Unauthorized();
        if (!user.OtpEnabled || string.IsNullOrEmpty(user.OtpSecret)) return BadRequest(new { message = "OTP not enabled" });

        if (!_otpService.ValidateTotp(user.OtpSecret, model.Code))
            return BadRequest(new { message = "Invalid OTP code." });

        user.OtpEnabled = false;
        user.OtpSecret = null;
        _context.Update(user);
        _context.SaveChanges();

        return Ok(new { message = "OTP disabled" });
    }
}

public class EnableOtpRequest { public string Code { get; set; } = null!; }
