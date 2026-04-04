// TODO: email verification endpoint
// Waiting for IAuthenticationService.VerifyEmailAsync to be implemented.
// Expected route: POST /api/auth/verify-email
// Expected body: { "token": "<guid>" }
// Expected responses:
//   200 OK  → email verified successfully
//   400 Bad Request → invalid or expired token
//   404 Not Found   → token not found
