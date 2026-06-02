# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Solution Structure

Photobox is a photography platform with three major parts:

- **`Photobox.Web/`** — ASP.NET Core 10 web API (hosts the REST API and serves the Vue SPA)
- **`photobox.web.client/`** — Vue 3 + TypeScript SPA (gallery UI, served via Vite proxy to the backend)
- **`Photobox.UI/`** — WPF desktop client for photographers (Windows only)
- **`Photobox.Lib/`** — Shared business logic, image processing, and the generated C# API client
- **`Photobox.UI.Lib/`** — Shared WPF logic including Canon EDSDK camera integration
- **`Photobox.Web.Uploader/`** — Console utility for bulk image uploads
- **`EDSKLib/`** — Canon EDSDK wrapper (camera control)

Libraries under test: `Photobox.Lib.Test/`, `Photobox.UI.Lib.Test/` (xUnit + NSubstitute + Shouldly).

## Commands

### Backend (.NET 10)

```powershell
dotnet build                                     # Build entire solution
dotnet run --project Photobox.Web/Photobox.Web   # Run web API (HTTPS on localhost)
dotnet test                                      # Run all tests
dotnet test --filter "FullyQualifiedName~ClassName"  # Run single test class
dotnet ef database update --project Photobox.Web/Photobox.Web  # Apply EF migrations
```

### Frontend (Vue 3)

```powershell
cd photobox.web.client
npm run dev          # Dev server on port 60005 (proxies /api to https://localhost)
npm run build        # Build + type-check
npm run type-check   # vue-tsc type checking only
npm run lint         # ESLint with auto-fix
```

### OpenAPI Client Regeneration

When backend API changes, regenerate both the TypeScript and C# clients from `Photobox.Web/Photobox.Web/`:

```powershell
nswag run nswag.json
```

This writes:
- `photobox.web.client/src/OpenApi/Client.ts` — TypeScript fetch client
- `Photobox.Lib/RestApi/Client.cs` — C# REST client used by UI and Uploader

`BaseClass.ts` in `photobox.web.client/src/OpenApi/` is the manually maintained fetch-interceptor extension point injected via `extensionCode` in `nswag.json`. Do not overwrite it.

### Docker

```powershell
docker-compose up    # Start PostgreSQL 17 + Photobox.Web
```

## Architecture

### API Layer

Controllers live in `Photobox.Web/Photobox.Web/Controllers/` and follow a service-injection pattern. All routes use `[Route("api/[controller]/[action]")]`. The Swagger spec is committed at `Photobox.Web/Photobox.Web/OpenAPI/swagger.json` and drives both generated clients. API docs are served via Scalar at `/scalar/v1` (not Swagger UI).

Most endpoints require `[Authorize(AuthenticationSchemes = "Identity.Bearer")]`. Identity endpoints are mapped via `app.MapGroup("api").MapIdentityApi<ApplicationUser>()`. The `GalleryController` is public (no auth) for gallery viewers.

**Hardware devices** authenticate using the `X-Hardware-Id` header (a registered PhotoBox GUID). This header is used by `EventController.GetGalleryCode` and `ImageController.UploadImage` to associate uploads with the correct event.

### Service Layer

Business logic lives in `Photobox.Web/Photobox.Web/Services/`. Each service is registered as Scoped via `ApplicationServiceCollectionExtensions`. FluentValidation validators in `Validators/` are auto-registered and called from controllers via `IValidator<T>`.

Models use `Guid.CreateVersion7()` for primary keys (monotonic, index-friendly). DTOs are split into request/response types in `DTOs/` and mapped with extension methods in `Mapping/`.

### Image Pipeline

1. A hardware device POSTs a JPEG to `/api/Image/UploadImage` with `X-Hardware-Id`.
2. `ImageService` downscales the image to ≤1000px width (SixLabors.ImageSharp, Rgb24, JPEG output) and stores both full and downscaled versions to Cloudflare R2 via `IStorageService`.
3. Pre-signed URLs are generated with a 30-minute TTL and cached in memory at 95% of that TTL.
4. Gallery viewers fetch image lists via `/api/Gallery/GetImagesFromGalleryCode/{code}` (6-digit event code, public endpoint) and receive pre-signed URLs directly.

### Frontend → Backend

The Vite dev server (`vite.config.ts`) proxies `/api` and `/users` to `https://localhost`. In production, ASP.NET Core serves the built Vue assets and handles routing.

The frontend calls the backend exclusively through the generated `Client.ts`. `src/services/api.ts` instantiates the client and overrides `fetch` to add `credentials: "same-origin"` and handle 401 responses via `handleUnauthorized()`.

**Auth flow** (`src/services/auth.ts` composable):
- Login → POST `/api/login` → stores `accessToken` + `refreshToken`
- Token injected per-request via `BaseClass.ts` fetch interceptor
- `initializeAuth()` called on app startup; refreshes the current user and sets reactive `currentUser` state
- 401/403 responses call `handleUnauthorized()` which clears the session

**Frontend routing** (`src/router/`): `/gallery/:code` for public gallery views; `/account` sub-routes for login, register, and post-login landing.

### Data & Storage

- **PostgreSQL 17** via EF Core: `AppDbContext` extends `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`
- Models: `ApplicationUser` → many `PhotoBox`es and `Event`s; `Event` → many `Image`s; `Event.UsedPhotoBoxId` → `PhotoBox` (no-action delete to avoid cycles)
- `Event.EventCode` has a unique index (6-character code)
- **Cloudflare R2** (S3-compatible, ForcePathStyle) for image blobs
- AWS credentials (`ServiceURL`, `AccessKey`, `SecretKey`) and the PostgreSQL connection string are configured via user secrets or environment variables
- Migrations are code-first; run `dotnet ef` commands from the `Photobox.Web` project

### Package Versioning

NuGet packages are centrally managed in `Directory.Packages.props` at the solution root. Add new packages there rather than directly in `.csproj` files.