## Plan: Implement JWT Authentication and Login Page

This plan describes how to implement a JWT-based authentication system for the Razor Pages application, including a login interface, token generation service, and middleware configuration for access control and redirection.

### Steps
1. Add `JwtSettings` (Key, Issuer, Audience, Duration) to [appsettings.json](D:\PRN222\FINAL\PRN222_Group5_ApartmentManagement\PRN222_ApartmentManagement\appsettings.json).
2. Create `IAuthService` and `AuthService` in [Services](D:\PRN222\FINAL\PRN222_Group5_ApartmentManagement\PRN222_ApartmentManagement\Services) to handle login verification using `BCrypt` and JWT generation.
3. Configure authentication in [Program.cs](D:\PRN222\FINAL\PRN222_Group5_ApartmentManagement\PRN222_ApartmentManagement\Program.cs) using `AddAuthentication` with both `JwtBearer` and `Cookie` schemes to support redirection for Razor Pages.
4. Implement the login interface in `Pages/Account/Login.cshtml` and its [code-behind](D:\PRN222\FINAL\PRN222_Group5_ApartmentManagement\PRN222_ApartmentManagement\Pages\Account\Login.cshtml.cs) to validate credentials and store the token in a secure cookie.
5. Create an `AccessDenied.cshtml` page and configure `LoginPath` and `AccessDeniedPath` in the authentication middleware.
6. Apply `[Authorize]` attributes to protected pages or folders (e.g., Admin folder) to trigger redirection if the user is unauthorized.

### Further Considerations
1. **Token Storage**: Since this is a Razor Pages app, should we store the JWT in a `Cookie` (easiest for SSR) or `LocalStorage` (requires more JS handling)? *Recommendation: Use Secure/HttpOnly Cookies.*
2. **Password Security**: Ensure `BCrypt.Net-Next` is installed for secure password hashing and verification.
3. **Role-based Access**: Create policies in `Program.cs` for `Admin`, `Staff`, and `Resident` based on the `UserRole` enum.

