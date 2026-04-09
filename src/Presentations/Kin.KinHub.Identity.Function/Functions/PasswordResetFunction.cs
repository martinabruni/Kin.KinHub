// TODO: password reset endpoints
// Waiting for IAuthenticationService.RequestPasswordResetAsync and ResetPasswordAsync to be implemented.
//
// Expected endpoints:
//
// 1. POST /api/auth/request-password-reset
//    Body: { "email": "<email>" }
//    Responses:
//      200 OK → reset token issued (returned in response for now; later send via email)
//      404 Not Found → email not registered
//
// 2. POST /api/auth/reset-password
//    Body: { "token": "<guid>", "newPassword": "<password>" }
//    Responses:
//      200 OK → password updated
//      400 Bad Request → invalid or expired token
//      404 Not Found → token not found
