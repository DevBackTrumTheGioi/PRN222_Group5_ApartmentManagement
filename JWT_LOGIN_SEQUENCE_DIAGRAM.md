# Sequence Diagram JWT Login

Tai lieu nay mo ta luong dang nhap JWT theo dung code hien tai trong project `PRN222_ApartmentManagement`.

## 1. Login bang JWT

```mermaid
sequenceDiagram
    autonumber
    actor U as User
    participant B as Browser
    participant L as LoginModel
    participant A as AuthService
    participant UR as UserRepository
    participant RR as RefreshTokenRepo
    participant LOG as ActivityLogService
    participant C as AuthCookieHelper
    participant DB as SQL Server

    U->>B: Nhap username/password
    B->>L: POST /Account/Login
    L->>A: LoginAsync(username, password, ip)
    A->>UR: GetByUsernameAsync(username)
    UR->>DB: SELECT User WHERE Username=? AND IsDeleted=0
    DB-->>UR: user / null
    UR-->>A: user

    alt Sai tai khoan hoac sai mat khau
        A->>LOG: LogLoginAsync(false)
        A-->>L: Success=false, ErrorMessage
        L-->>B: Render lai Login va hien loi
    else Tai khoan inactive
        A->>LOG: LogLoginAsync(false, Account is inactive)
        A-->>L: IsInactiveAccount=true
        L-->>B: Set Session PendingUserId
        L-->>B: Redirect /Account/VerifyPhone
    else Dang nhap hop le
        A->>A: Verify BCrypt password
        A->>A: GenerateJwtToken(claims: userId, username, role, fullName)
        A->>RR: AddAsync(TokenHash=SHA256(refreshToken))
        RR->>DB: INSERT UserRefreshTokens
        A->>UR: UpdateAsync(LastLogin, UpdatedAt)
        UR->>DB: UPDATE Users
        A->>LOG: LogLoginAsync(true)
        A-->>L: TokenPairResult + User
        L->>C: SetAuthCookies(HttpContext, tokens)
        C-->>B: Set-Cookie AccessToken
        C-->>B: Set-Cookie RefreshToken
        L->>L: ResolvePostLoginUrl(returnUrl, role)
        L-->>B: 302 Redirect theo role hoac returnUrl
    end
```

## 2. Request sau khi da login

```mermaid
sequenceDiagram
    autonumber
    actor U as User
    participant B as Browser
    participant MW as JwtCookieRefreshMiddleware
    participant A as AuthService
    participant RR as RefreshTokenRepo
    participant C as AuthCookieHelper
    participant J as JwtBearerEvents
    participant P as Protected Razor Page
    participant DB as SQL Server

    U->>B: Mo trang can dang nhap
    B->>MW: GET /Resident/... kem cookies

    alt Co Authorization Bearer
        MW-->>J: Bo qua refresh bang cookie
    else Khong co Bearer header
        MW->>MW: Doc AccessToken va RefreshToken tu cookie
        alt AccessToken con hop le
            MW-->>J: Cho request di tiep
        else AccessToken thieu hoac het han
            MW->>A: RefreshTokenAsync(refreshToken, ip)
            A->>RR: GetByTokenHashAsync(SHA256(refreshToken))
            RR->>DB: SELECT RefreshToken INCLUDE User
            alt Refresh token invalid, revoked, expired, hoac user inactive
                A-->>MW: Success=false
                MW->>C: ClearAuthCookies()
            else Refresh token hop le
                A->>RR: AddAsync(new refresh token)
                RR->>DB: INSERT new refresh token
                A->>RR: UpdateAsync(old token: RevokedAt, ReplacedByTokenHash)
                RR->>DB: UPDATE old refresh token
                A-->>MW: new AccessToken + RefreshToken
                MW->>C: SetAuthCookies(new tokens)
                MW->>MW: context.Items[AccessToken] = newAccessToken
            end
        end
    end

    MW->>J: UseAuthentication()
    J->>J: OnMessageReceived lay token
    J->>J: Thu tu lay token la Query cho SignalR, context.Items, Authorization header, AccessToken cookie
    J->>J: Validate signature, issuer, audience, lifetime

    alt JWT hop le
        J->>DB: SELECT User theo claim NameIdentifier
        alt User ton tai va IsActive=true
            J-->>P: Gan HttpContext.User
            P-->>B: 200 OK
        else User bi khoa hoac khong hop le
            J-->>B: Redirect /Account/Login?returnUrl=...
        end
    else Khong co token hoac token sai
        J-->>B: Redirect /Account/Login?returnUrl=...
    end
```

## 3. Tom tat ngan gon

- Login page goi `IAuthService.LoginAsync`.
- `AuthService` kiem tra user, password, trang thai active.
- Neu hop le, he thong tao `AccessToken` va `RefreshToken`.
- `RefreshToken` duoc hash roi luu DB.
- Ca 2 token duoc set vao cookie `HttpOnly`.
- Moi request sau do di qua `JwtCookieRefreshMiddleware` truoc.
- Neu `AccessToken` het han va `RefreshToken` con hop le, he thong tu refresh token moi.
- Sau do `JwtBearer` validate token va tai lai user tu DB de xac nhan user van con active.

## 4. File code lien quan

- `PRN222_ApartmentManagement/Pages/Account/Login.cshtml.cs`
- `PRN222_ApartmentManagement/Services/Implementations/AuthService.cs`
- `PRN222_ApartmentManagement/Services/JwtCookieRefreshMiddleware.cs`
- `PRN222_ApartmentManagement/Utils/AuthCookieHelper.cs`
- `PRN222_ApartmentManagement/Program.cs`

## 5. Dang ky khuon mat xac thuc

```mermaid
sequenceDiagram
    autonumber
    actor R as Resident
    participant B as Browser
    participant F as face-api.js
    participant P as Resident Face Register Page
    participant S as FaceAuthService
    participant UR as UserRepository
    participant LOG as ActivityLogService
    participant DB as SQL Server

    R->>B: Mo trang /Resident/FaceAuth/Register
    B->>P: GET Register page
    P-->>B: HTML + script face-api.js

    B->>F: Load face models from CDN
    B->>B: navigator.mediaDevices.getUserMedia(video)
    F->>B: detectSingleFace + face landmarks + descriptor

    alt Camera nhan dien duoc khuon mat
        F-->>B: currentDescriptor
        B-->>R: Bat nut Xac nhan khuon mat
        R->>B: Bam Xac nhan khuon mat
        B->>P: POST FaceDescriptorString
        P->>P: Doc userId tu claims
        P->>S: RegisterFaceAsync(userId, faceDescriptor)
        S->>S: ValidateDescriptor()
        S->>S: GetResidentByIdAsync(userId)
        S->>UR: GetActiveByIdAsync(userId)
        UR->>DB: SELECT User by UserId
        DB-->>UR: Resident / null
        UR-->>S: user

        alt Descriptor khong hop le hoac khong tim thay resident
            S-->>P: Success=false, ErrorMessage
            P-->>B: Render lai page va hien loi
        else Hop le
            S->>UR: UpdateAsync(FaceDescriptor, IsFaceRegistered=true, UpdatedAt)
            UR->>DB: UPDATE Users
            S->>LOG: LogCustomAsync(RegisterFace)
            LOG->>DB: INSERT ActivityLogs
            S-->>P: Success=true
            P-->>B: Redirect /Resident/FaceAuth/Status
        end
    else Khong nhan dien duoc khuon mat
        F-->>B: No face detected
        B-->>R: Hien canh bao va khoa nut submit
    end
```

Ghi chu:

- Neu staff ho tro dang ky tai quay, page se la `BQL_Staff/FaceAuth/Register`.
- Khi do service duoc goi la `RegisterFaceForResidentAsync(actorUserId, residentId, faceDescriptor)`.
- Phan validate descriptor va cap nhat `User.FaceDescriptor` van giong nhau.

## 6. Xac thuc khuon mat tai diem amenity

```mermaid
sequenceDiagram
    autonumber
    actor ST as BQL Staff
    participant B as Browser
    participant F as face-api.js
    participant P as AmenityAccess Page
    participant S as FaceAuthService
    participant RAA as ResidentApartmentAccessService
    participant HR as FaceAuthHistoryRepo
    participant LOG as ActivityLogService
    participant DB as SQL Server

    ST->>B: Mo trang /BQL_Staff/FaceAuth/AmenityAccess
    B->>P: GET page + AmenityId
    P->>DB: SELECT active amenities
    P->>S: GetResidentSummariesAsync()
    S->>DB: SELECT resident summaries
    P-->>B: HTML + danh sach amenity + script camera

    ST->>B: Chon amenity
    B->>F: Load face models
    B->>B: Mo camera
    F->>B: detectSingleFace + descriptor

    alt Quet duoc khuon mat
        ST->>B: Bam quet/check-in
        B->>P: POST OnPostScan(AmenityId, FaceDescriptorString, DeviceInfo)
        P->>S: ValidateAmenityAccessAsync(amenityId, descriptor, ip, deviceInfo)
        S->>DB: SELECT Amenity active by AmenityId

        alt Amenity khong ton tai hoac descriptor loi
            S-->>P: AccessGranted=false, Message=error
            P-->>B: Hien ket qua tu choi
        else Du lieu hop le
            S->>DB: SELECT active residents co FaceDescriptor
            S->>S: Tinh EuclideanDistance voi tung resident
            S->>S: Chon resident co khoang cach nho nhat

            alt Khong tim thay resident match hoac vuot threshold 0.60
                S-->>P: AccessGranted=false, IsMatchFound=false
                P-->>B: Hien thong bao khong tim thay khuon mat phu hop
            else Match thanh cong
                S->>RAA: HasAnyActiveApartmentAsync(residentId)
                RAA->>DB: Kiem tra cu dan con can ho hoat dong

                alt Cu dan chua co can ho hop le
                    S->>HR: AddAsync(FaceAuthHistory failed)
                    HR->>DB: INSERT FaceAuthHistory
                    S-->>P: AccessGranted=false
                    P-->>B: Tu choi truy cap amenity
                else Ngoai gio hoat dong
                    S->>HR: AddAsync(FaceAuthHistory failed)
                    HR->>DB: INSERT FaceAuthHistory
                    S-->>P: AccessGranted=false
                    P-->>B: Thong bao amenity dang ngoai gio
                else Amenity mo tu do, khong can booking
                    S->>HR: AddAsync(FaceAuthHistory success)
                    HR->>DB: INSERT FaceAuthHistory
                    S-->>P: AccessGranted=true
                    P-->>B: Cho phep vao amenity
                else Amenity can booking
                    S->>DB: SELECT AmenityBookings confirmed trong khung gio hien tai
                    alt Khong co booking hop le
                        S->>HR: AddAsync(FaceAuthHistory failed)
                        HR->>DB: INSERT FaceAuthHistory
                        S-->>P: AccessGranted=false
                        P-->>B: Tu choi vi khong co booking hop le
                    else Co booking hop le
                        S->>HR: AddAsync(FaceAuthHistory success)
                        HR->>DB: INSERT FaceAuthHistory
                        S-->>P: AccessGranted=true, BookingWindowLabel
                        P-->>B: Cho phep vao amenity
                    end
                end
            end
        end
    else Camera khong nhan duoc mat
        F-->>B: No face detected
        B-->>ST: Canh bao va co the chuyen sang check-in thu cong
    end
```

Ghi chu:

- Luong tren la luong quet khuon mat chinh.
- Neu camera loi hoac khong match, staff co the dung `OnPostManualAsync`.
- Khi check-in thu cong, service se goi `ValidateAmenityAccessManualAsync(...)` va co them log `ManualAmenityCheckInApproved` hoac `ManualAmenityCheckInRejected`.
