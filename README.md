# AspNetControllerApiBoilerPlate – ASP.NET Controller-Based API Server

## 🔐 Authentication

AspNetControllerApiBoilerPlate uses **ASP.NET Identity Framework** with **JWT-based authentication**, leveraging **HTTP-only cookies** for
secure session management.

### How it works:

- Upon login, a **JWT token** is generated containing minimal user information.
- This token is **stored in an HTTP-only cookie**.
- Each subsequent request automatically includes this cookie.
- The **Identity system validates the JWT** on every request to authorize access.

### Enabled Security Features:

- ✅ Email-based account confirmation
- ✅ Two-Factor Authentication (2FA) using a time-limited code sent via email

---

## ⚙️ Environment Configuration

To run the application, you must define an `appsettings.<Environment>.json` file that corresponds to your current
environment (e.g., `Development`, `Production`).  
You can use `appsettings.example.json` as a reference for setting up your configuration.

Make sure to define:

- JWT settings (issuer, audience, lifetime)
- Email SMTP settings for confirmation and 2FA
- Database connection strings
- RSA key paths or inline keys

---

## 🔐 JWT with Asymmetric Encryption (RSA)

This application uses **asymmetric encryption (RSA)** for JWT token signing and validation.

### Generating RSA Keys

You’ll need both a **private key** (for signing JWTs) and a **public key** (for validation).

#### Option 1: Generate using OpenSSL

```bash
# Generate private key (2048 bits)
openssl genpkey -algorithm RSA -out private.key -pkeyopt rsa_keygen_bits:2048

# Extract public key from private key
openssl rsa -pubout -in private.key -out public.key
```

Then convert the generated keys to XML format and add them to the appsettings.json file.