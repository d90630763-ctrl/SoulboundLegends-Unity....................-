# Google OIDC setup

1. Create/configure the Google OAuth client in the Google Cloud project.
2. Use a Web Application OAuth client appropriate for the backend authentication flow.
3. Put only the Google client ID in the backend configuration.
4. Keep any Google client secret on the server only.
5. Configure the client application to obtain a Google ID token and a cryptographic nonce.
6. Send the ID token and the original nonce to `POST /api/v1/auth/login/google`.
7. The backend validates Google issuer, audience, lifetime, signing key and nonce.
8. For account linking, use the same verification path while the user already has an authenticated SL session.

Google credentials are external developer-supplied configuration and are intentionally not included in this ZIP.
