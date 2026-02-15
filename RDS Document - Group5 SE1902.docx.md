![][image1]

# **Requirement & Design Specification**

**Recruitment System (RS)**

**Version: 1.0**

– Hanoi, June 2025 –

# Record of Changes {#record-of-changes}

| Version | Date | A\*M, D | In charge | Change Description |
| :---: | :---: | :---: | ----- | ----- |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |
|  |  |  |  |  |

\*A \- Added M \- Modified D \- Deleted

Contents  
[Record of Changes	2](#record-of-changes)

[I. Overview	4](#i.-overview)

[1\. System Context	4](#1.-product-vision)

[2\. User Requirements	4](#4.-user-requirements)

[2.1 Actors	4](#4.1-actors-list)

[2.2 Diagrams	5](#4.2-diagrams)

[2.3 Descriptions	6](#2.3-descriptions-\(cai-nay-sau-cho-len-4.3-nha\))

[3\. System Functionalities	6](#heading=h.8z4ouk8ti42e)

[3.1 Screens Flow	6](#heading=h.dy8mtb1wwii4)

[3.2 Screen Authorization	6](#heading=h.lc7wtohl5tei)

[3.3 Non-UI Functions	7](#heading=h.zg7dim1ex8ba)

[II. Functional Requirements	8](#heading=h.cnhrdtya564t)

[1\. Feature Name1	8](#heading=h.psw7byw6xvo2)

[1.1 SubFeature Name1.1	8](#heading=h.q0dji33jae5z)

[1.2 SubFeature Name1.2	8](#heading=h.g40effc4hh8f)

[2\. User Authentication	8](#heading=h.bc7gs5i4jglg)

[2.1 User Register	8](#heading=h.11em27fhq5kp)

[2.2 User Login	8](#heading=h.vhbqy5hfcfyf)

[2.3 Password Reset	8](#heading=h.gatp1tt58ugi)

[2.4 User Profile	8](#heading=h.9k1z7jyvfyxu)

[3\. System Administration	9](#heading=h.atgr3xvcg8s7)

[3.1 Master Data	9](#heading=h.i3c1l8dvlejb)

[3.2 User Management	10](#heading=h.7pz3w4i3v5ge)

[III. System Design	10](#heading=h.txryja6zn7nj)

[1\. Database Design	10](#heading=h.932v51zb9v3m)

[1.1 Database Schema	10](#heading=h.s25igy637mvo)

[1.2 Table Descriptions	10](#heading=h.6tzty2c5uy9l)

[2\. Code Packages	11](#heading=h.bxiir69ipagv)

[2.1 Package Diagram	11](#heading=h.maap4eir6e85)

[2.2 Package Descriptions	11](#heading=h.87jkr34dc2hi)

# **I. Overview** {#i.-overview}

## **1\. Product Vision**  {#1.-product-vision}

The Apartment Management System aims to revolutionize property management by delivering a unified digital ecosystem that bridges operational efficiency with enhanced resident satisfaction. By automating administrative tasks like billing and contract management, improving the resident experience through a self-service portal, and fostering a secure, connected community with integrated communication and security tools, the platform empowers property managers and residents alike to enjoy a seamless, modern living environment.

## **2\. Product Context** 

## 

## ![][image2]

## 

## 

## **3\. Major Features**

1. Centralized Administration: Comprehensive tools for admins to manage users (staff, residents), roles, and system configurations like service types and prices.

2. Resident Lifecycle Management: End-to-end management of resident data, from contract creation and move-in to daily living (vehicles, cards) and eventual move-out.

3. Automated Billing & Payments: Streamlined financial operations including monthly meter readings, auto-generated invoices, and payment tracking history.

4. Facility & Service Management: Digital booking for amenities (pools, gyms) and subscription management for apartment-specific services.

5. Communication & Support: Integrated channels for announcements, maintenance requests, and direct messaging between residents and staff.

6. Security & Access Control: Management of resident access cards, vehicle registration, and visitor logging for enhanced building security.

## **4\. User Requirements** {#4.-user-requirements}

### 4.1 Actors List {#4.1-actors-list}

| Actor | Description |
| :---- | :---- |
| Administrator (Admin) | System superuser responsible for user accounts, roles, and technical system configuration. |
| Manager | Property manager responsible for business operations, contracts, pricing, and financial oversight. |
| Staff | Operational personnel (Receptionist, Security, Maintenance) handling day-to-day tasks like visitor logs, parcels, and basic requests. |
| Resident | Tenants or owners living in the apartments using the resident portal. |
| System | Automated background processes (cron jobs, billing cycles, notifications). |
| Visitor | Guests checking in via reception or using QR codes for access. |
| Payment Gateway | Facilitates secure online transactions by authorizing and processing various payment methods between merchants and customers. |
| Mail Gateway | A specialized server that manages, filters, and routes email traffic to ensure secure and reliable delivery. |
| RFID Reader | A hardware device used to capture and read data stored on RFID tags via radio waves for tracking or access control. |
| Face Recognition Service | An AI-powered service that identifies or verifies individuals by analyzing and comparing unique facial patterns from images. |

### 4.2 Diagrams {#4.2-diagrams}

#### *4.1 Administrator (Admin)*

* Manage Users \<---extend--- Block User  
* Manage Roles

#### *4.2 Manager (Property Manager)*

* Manage Residents \<---extend--- Update Resident Info  
* Manage Residents \<---extend--- Process Move Out  
* Manage Service Types \<---extend--- Edit Service Type  
* Manage Service Prices  
* Manage Amenities \<---extend--- Deactivate Amenity  
* Create Contract  
* Terminate Contract  
* Generate Invoice  
* Post Announcement  
* Classify/Assign Ticket  
* View Apartment List

#### *4.3 Staff (Reception/Security)*

* View Apartment List \<---extend--- View Apartment Details  
* Manage Residents (Basic data entry)  
* Manage Resident Cards \<---extend--- Lock Card  
* Manage Resident Cards \<---extend--- Extend Card  
* Manage Vehicles \<---extend--- Remove Vehicle  
* Register Visitor \<---extend--- Visitor Check-out  
* Record Meter Readings \<---extend--- Bulk Import Readings  
* Register Apt Services \<---extend--- Cancel Service  
* Process Payment (Cash/Desk)  
* Execute Ticket  
* Manage Parcels \<---include--- Notify Resident  
* Send Message

#### *4.4 Resident*

* User Login \<---extend--- Forgot Password  
* View Dashboard  
* View Invoices  
* View Payment History  
* Submit Ticket  
* Register Visitor (Pre-register)  
* Book Amenity  
* View Contracts  
* Send Message  
* View Notifications

#### *4.5 All Users (Common Actions)*

* User Login  
* Update Profile  
* Change Password  
* View Notifications

#### 

## 5\. Assumptions & Dependencies

### 5.1 Assumptions

* Internet Access: All users (Admin, Staff, Residents) have reliable internet access to use the web-based system.  
* User Competency: Users possess basic computer literacy and can navigate standard web interfaces.  
* Data Availability: Historical data for initial system population (e.g., existing contracts, resident list) is available in a compatible format.  
* Hardware Devices: Staff stations are equipped with necessary peripherals (QR scanners, card readers) if physical check-in features are utilized.  
* Email Validity: All users provide valid email addresses for account recovery and notifications.

### 5.2 Dependencies

* Browser Support: The system depends on modern web browsers (Chrome, Edge, Firefox, Safari) with JavaScript enabled.  
* Third-Party Services:  
  * Email Service: Relies on an SMTP server (e.g., Gmail, SendGrid) for sending notifications.  
  * Payment Gateway: Depends on stable integration with payment providers (e.g., VNPay, Momo) for transaction processing (or their sandbox/mock environments).  
* Infrastructure: Dependent on the hosting environment (Azure/AWS/Local Server) availability and database server (SQL Server) uptime.

## 6\. Limitations and Exclusions

### 6.1 Limitations

* Platform: The system is a Web Application; no native mobile app (iOS/Android) is included in this phase, though the web UI is responsive.  
* Offline Access: The application requires an active network connection; no offline mode or local data synchronization is currently supported.  
* Scalability: Optimized for a single apartment complex management. Multi-tenant architecture (SAAS for multiple diverse properties) is not currently implemented.  
* Payment Processing: Real money transactions may be simulated using sandbox environments due to compliance/licensing restrictions in a project context.

### 6.2 Exclusions

* Hardware Integration: Direct integration with physical building infrastructure (e.g., automatic barrier arms, elevator control systems, IoT cameras) is out of scope.  
* Accounting Software Integration: No direct API sync with external accounting software (e.g., QuickBooks, SAP); financial reports are generated internally.  
* Real-time Chat: Messaging is asynchronous (inbox-style) rather than a real-time WebSocket-based instant messenger, unless specified purely as a signalR implementation.  
* Biometric Data: Collection and storage of biometric data (fingerprints, face ID) are excluded for privacy and complexity reasons.

# **II. Use Case Specifications**

## UC-01: User Login

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-01 User Login |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | All Users |
| **Secondary Actors** | System |
| **Trigger** | User accesses the application URL or clicks "Login". |
| **Description** | Allows users to authenticate into the system using their credentials to access role-specific features. |
| **Preconditions** | User must have a registered account. |
| **Postconditions** | User is authenticated and redirected to the appropriate dashboard. |
| **Normal Flow** | 1\. User enters username and password. 2\. User clicks "Login". 3\. System validates credentials against database. 4\. System creates session/token. 5\. System redirects user to Dashboard. |
| **Alternative Flows** | **A1. Forgot Password**: User clicks "Forgot Password" \-\> System initiates recovery process. |
| **Exceptions** | **E1. Invalid Credentials**: System shows "Invalid username or password" error. \*\*E2. Account Locked\*\*: If account is inactive, system denies access. |
| **Priority** | High |
| **Frequency of Use** | Every session |
| **Business Rules** | Password must be hashed. 5 failed attempts may lock account (optional). |
| **Assumptions** | None |

## UC-02: View Dashboard

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-02 View Dashboard |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | All Users |
| **Secondary Actors** | None |
| **Trigger** | Successful login or clicking "Dashboard" in menu. |
| **Description** | Displays a summary of relevant information based on user role (e.g., Unpaid bills for Residents, Pending requests for Staff). |
| **Preconditions** | User is logged in. |
| **Postconditions** | Dashboard rendered with up-to-date data. |
| **Normal Flow** | 1\. System identifies user role. 2\. System queries relevant summary stats (Total residents, Pending requests, etc.). 3\. System renders the dashboard view. |
| **Alternative Flows** | None |
| **Exceptions** | **E1. Data Load Fail**: Widgets show "Error loading data". |
| **Priority** | Medium |
| **Frequency of Use** | Daily |
| **Business Rules** | Residents only see their own data stats. Admin sees system-wide stats. |
| **Assumptions** | None |

## UC-03: Update Profile

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-03 Update Profile |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | All Users |
| **Secondary Actors** | None |
| **Trigger** | User navigates to profile settings. |
| **Description** | Allows users to update their contact information (Phone, Email, Avatar). |
| **Preconditions** | User is logged in. |
| **Postconditions** | User profile is updated in database. |
| **Normal Flow** | 1\. User edits allowable fields (Phone, Email). 2\. User uploads new avatar (optional). 3\. User clicks "Save". 4\. System validates format. 5\. System saves changes. |
| **Alternative Flows** | None |
| **Exceptions** | **E1. Invalid Email**: System rejects format. |
| **Priority** | Low |
| **Frequency of Use** | Monthly |
| **Business Rules** | Username cannot be changed by user. |
| **Assumptions** | None |

## UC-04: Change Password

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-04 Change Password |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | All Users |
| **Secondary Actors** | None |
| **Trigger** | User decides to change security credentials. |
| **Description** | Securely updates the user authentication password. |
| **Preconditions** | User knows current password. |
| **Postconditions** | Password hash is updated. |
| **Normal Flow** | 1\. User enters Current Password. 2\. User enters New Password and Confirm Password. 3\. System verifies current password matches DB. 4\. System matches new password confirmation. 5\. System updates password hash. |
| **Alternative Flows** | None |
| **Exceptions** | **E1. Wrong Current Password**: System denies change. |
| **Priority** | Medium |
| **Frequency of Use** | Quarterly |
| **Business Rules** | New password must meet complexity requirements. |
| **Assumptions** | None |

## UC-05: Manage Users

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-05 Manage Users |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | Admin needs to onboard staff or block an account. |
| **Description** | Create, update, lists, and deactivate Staff or Admin accounts. |
| **Preconditions** | Logged in as Admin. |
| **Postconditions** | User list updated. |
| **Normal Flow** | 1\. Admin views list of users. 2\. Admin clicks "Create User". 3\. Admin fills account details and selects Role. 4\. System creates account. 5\. System sends default credentials (or sets them). |
| **Alternative Flows** | **A1. Block User**: Admin toggles "IsActive" to false. |
| **Exceptions** | **E1. Duplicate User**: Username exists. |
| **Priority** | High |
| **Frequency of Use** | Monthly |
| **Business Rules** | Admin cannot delete their own account. |
| **Assumptions** | None |

## UC-06: Manage Roles

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-06 Manage Roles |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | Need to adjust permissions (if dynamic). |
| **Description** | View and configure system roles. (Note: In this system, roles are likely static enums, so this enables viewing role definitions). |
| **Preconditions** | Admin access. |
| **Postconditions** | Role config updated. |
| **Normal Flow** | 1\. Admin views roles. 2\. Admin views associated permissions. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Low |
| **Frequency of Use** | Rare |
| **Business Rules** | Standard roles (Admin, Resident, Staff) cannot be deleted. |
| **Assumptions** | Roles are predefined in code. |

## UC-07: Manage Service Types

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-07 Manage Service Types |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | New service offered (e.g., charging station). |
| **Description** | Create and manage categories of services available in the apartment complex. |
| **Preconditions** | Admin access. |
| **Postconditions** | Service type added. |
| **Normal Flow** | 1\. Admin navigates to Service Types. 2\. Admin adds new type (Name, Unit). 3\. System saves type. |
| **Alternative Flows** | **A1. Edit**: Update name or unit. |
| **Exceptions** | **E1. Duplicate Name**: Error displayed. |
| **Priority** | Medium |
| **Frequency of Use** | Yearly |
| **Business Rules** | Cannot delete service type if used in invoices. |
| **Assumptions** | None |

## UC-08: Manage Service Prices

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-08 Manage Service Prices |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | Utility rate change. |
| **Description** | Set the unit price for services valid from a specific date. |
| **Preconditions** | Service Type exists. |
| **Postconditions** | New price record created. |
| **Normal Flow** | 1\. Admin selects transaction. 2\. Admin enters new Price. 3\. Admin sets Effective Date. 4\. System saves price history. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | High |
| **Frequency of Use** | Monthly/Quarterly |
| **Business Rules** | Prices are historical; old prices are kept for past invoices. |
| **Assumptions** | None |

## UC-09: Manage Amenities

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-09 Manage Amenities |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | New facility opens. |
| **Description** | Configure bookable amenities (Pool, BBQ, Tennis). |
| **Preconditions** | Admin access. |
| **Postconditions** | Amenity listed for booking. |
| **Normal Flow** | 1\. Admin adds Amenity. 2\. Enters location, capacity, optional price. 3\. Saves. |
| **Alternative Flows** | **A1. Deactivate**: Mark as "Maintenance". |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Rare |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-10: View Apartment List

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-10 View Apartment List |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin, Staff |
| **Secondary Actors** | None |
| **Trigger** | User selects "Apartment Management" from the navigation menu. |
| **Description** | Allows the actor to view a list of all apartments, check their current status (Occupied, Available, Maintenance), and filter/search for specific units. |
| **Preconditions** | 1\. User is logged in as Admin or Staff. 2\. Apartment data exists in the system. |
| **Postconditions** | User sees the requested apartment information. |
| **Normal Flow** | 1\. User navigates to the "Apartments" page. 2\. System retrieves and displays a paginated list of apartments including Number, Floor, Block, and Status. 3\. User reviews the information. 4\. User inputs search criteria (e.g., Block A) or selects a status filter. 5\. System updates the list to match criteria. |
| **Alternative Flows** | **A1. View Details**: Steps 1-2. User clicks "Details" on a row. System shows detailed info (residents, history). |
| **Exceptions** | **E1. No Data**: If no apartments exist, system displays "No records found". \*\*E2. Database Error\*\*: System displays an error message and logs the incident. |
| **Priority** | High |
| **Frequency of Use** | Daily |
| **Business Rules** | Sensitive resident details are only visible in the Detail view, not the list view. |
| **Other Information** | Used as an entry point for other actions (e.g., updating resident list). |
| **Assumptions** | User has a device with internet access. |

## UC-11: Manage Residents

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-11 Manage Residents |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin, Staff |
| **Secondary Actors** | System (creates login account) |
| **Trigger** | A new resident moves in, updates info, or moves out. |
| **Description** | Create new resident profiles, update existing personal information, or process move-out procedures. |
| **Preconditions** | 1\. User is authenticated. 2\. Target apartment exists. |
| **Postconditions** | Resident record is created/updated/deactivated in the database. |
| **Normal Flow** | **(Add New)** 1\. User selects "Add Resident". 2\. User enters personal info (Name, DOB, ID Card) and selects Apartment. 3\. System validates input. 4\. System saves resident and creates a linked User account. 5\. System displays success message. |
| **Alternative Flows** | **A1. Update Resident**: Select resident \-\> Edit \-\> Save changes. \*\*A2. Move Out\*\*: Select resident \-\> Click "Move Out" \-\> Enter date \-\> System sets status to Inactive. |
| **Exceptions** | **E1. Duplicate ID**: If ID Card number exists, system creates an error alert. \*\*E2. Validation Fail\*\*: If required fields are missing, system highlights them. |
| **Priority** | High |
| **Frequency of Use** | Weekly |
| **Business Rules** | 1\. Every resident must be linked to an apartment via `ApartmentId`. 2\. ID Card Number must be unique active residents. |
| **Other Information** | N/A |
| **Assumptions** | Resident has a device with internet access. |

## UC-12: Manage Resident Cards

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-12 Manage Resident Cards |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | None |
| **Trigger** | Resident requests a new card or reports a lost card. |
| **Description** | Issue access cards (Parking, Elevator, Entry) to residents and manage their status (Active, Locked). |
| **Preconditions** | 1\. Resident account must exist and be active. |
| **Postconditions** | Card is registered in the system and linked to the resident. |
| **Normal Flow** | 1\. Staff navigates to resident detail page. 2\. Staff selects "Issue Card". 3\. Staff scans or enters Card Number and selects Type. 4\. System checks if card number is unique. 5\. System saves the card record. |
| **Alternative Flows** | **A1. Lock Card**: Staff finds lost card \-\> Changes status to "Lost/Locked". \*\*A2. Extend Card\*\*: Staff updates expiry date. |
| **Exceptions** | **E1. Card in Use**: If card number is assigned to another person, system denies entry. |
| **Priority** | Medium |
| **Frequency of Use** | Weekly |
| **Business Rules** | One resident can hold multiple cards, but a single card belongs to only one resident. |
| **Other Information** | Integration with physical access control system is out of scope (manual sync assumed). |
| **Assumptions** | Card hardware is available. |

## UC-13: Manage Vehicles

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-13 Manage Vehicles |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | None |
| **Trigger** | Resident buys a new vehicle or changes vehicle. |
| **Description** | Register resident vehicles (Car, Motorbike) for parking management and security control. |
| **Preconditions** | Resident must be active. |
| **Postconditions** | Vehicle data is stored. |
| **Normal Flow** | 1\. Staff selects "Register Vehicle". 2\. Staff enters License Plate, Type, Brand, Color. 3\. Staff links vehicle to Resident. 4\. System validates License Plate uniqueness. 5\. System saves information. |
| **Alternative Flows** | **A1. Remove Vehicle**: Staff deletes or deactivates vehicle record when resident sells it. |
| **Exceptions** | **E1. Duplicate Owner**: If license plate is already registered, system shows error. |
| **Priority** | Medium |
| **Frequency of Use** | Weekly |
| **Business Rules** | Parking fees may be generated automatically based on registered vehicle count (handled in Billing UC). |
| **Other Information** | N/A |
| **Assumptions** | Physical parking slot allocation is managed separately or manually. |

## UC-14: Register Visitor

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-14 Register Visitor |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff, Resident |
| **Secondary Actors** | None |
| **Trigger** | A guest arrives at the building or Resident pre-registers a guest. |
| **Description** | Log visitor entry/exit details for security purposes. |
| **Preconditions** | Target apartment must be valid. |
| **Postconditions** | Visitor log entry created. |
| **Normal Flow** | **(Staff Check-in)** 1\. Visitor arrives at reception. 2\. Staff asks for destination apartment and ID. 3\. Staff enters Visitor Name, ID, Apartment No. 4\. System records entry time. 5\. Staff issues temporary pass (optional). |
| **Alternative Flows** | **A1. Resident Pre-register**: Resident logs in \-\> Submits visitor info \-\> System generates QR Code \-\> Visitor scans QR at gate. \*\*A2. Check-out\*\*: Staff finds visitor record \-\> Clicks "Check Out" \-\> System records exit time. |
| **Exceptions** | **E1. Blacklist**: If visitor is blacklisted (future feature), system alerts security. |
| **Priority** | High |
| **Frequency of Use** | Continuous (Daily) |
| **Business Rules** | Visitors must provide valid ID or be pre-approved by a resident. |
| **Other Information** | N/A |
| **Assumptions** | Security staff is present 24/7. |

## UC-15: Create Contract

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-15 Create Contract |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | Resident |
| **Trigger** | New tenant agrees to lease. |
| **Description** | Create a new housing contract linking an apartment to a resident (tenant/owner). |
| **Preconditions** | Apartment is available. Resident exists. |
| **Postconditions** | Contract created, Apartment status becomes "Occupied". |
| **Normal Flow** | 1\. Admin selects "New Contract". 2\. Selects Apartment and Resident. 3\. Enters Start/End Date, Rent, Deposit. 4\. Uploads contract file (optional). 5\. System saves contract. 6\. System updates apartment status to "Occupied". |
| **Alternative Flows** | None |
| **Exceptions** | **E1. Apt Occupied**: System prevents new contract on occupied unit. |
| **Priority** | High |
| **Frequency of Use** | Weekly |
| **Business Rules** | Contract must have distinct start/end dates. |
| **Assumptions** | None |

## UC-16: Terminate Contract

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-16 Terminate Contract |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | Resident moves out or contract expires. |
| **Description** | End an active contract and release the apartment. |
| **Preconditions** | Active contract exists. |
| **Postconditions** | Contract status "Terminated". Apartment status "Available". |
| **Normal Flow** | 1\. Admin loads active contract. 2\. Clicks "Terminate". 3\. Enters Termination Date and Reason. 4\. System updates statuses. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Weekly |
| **Business Rules** | Termination date cannot be before start date. |
| **Assumptions** | Financial settlement handled in Invoicing. |

## UC-17: View Contracts

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-17 View Contracts |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Resident |
| **Secondary Actors** | None |
| **Trigger** | Resident checks lease details. |
| **Description** | Allows resident to view their own active and past contracts. |
| **Preconditions** | Resident logged in. |
| **Postconditions** | Contract details shown. |
| **Normal Flow** | 1\. Resident navigates to "My Contracts". 2\. System displays list linked to user. 3\. Resident views details. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Low |
| **Frequency of Use** | Rare |
| **Business Rules** | Residents see only their own contracts. |
| **Assumptions** | None |

## UC-18: Record Meter Readings

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-18 Record Meter Readings |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | None |
| **Trigger** | Monthly utility check. |
| **Description** | Input current index for water/electricity meters for each apartment. |
| **Preconditions** | Previous reading exists (or 0 for new). |
| **Postconditions** | Reading saved. Consumption calculated. |
| **Normal Flow** | 1\. Staff selects month/year. 2\. Staff selects Apartment & Service Type. 3\. Staff enters Current Reading. 4\. System calculates Consumption (Current \- Previous). 5\. System saves reading. |
| **Alternative Flows** | **A1. Bulk Import**: Upload Excel file. |
| **Exceptions** | **E1. Lower Reading**: If current \< previous, system warns (meter rollover logic). |
| **Priority** | High |
| **Frequency of Use** | Monthly |
| **Business Rules** | Readings form basis for invoice calculation. |
| **Assumptions** | None |

## UC-19: Register Apt Services

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-19 Register Apt Services |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | Resident |
| **Trigger** | Resident requests extra service (Internet, Cleaning). |
| **Description** | Link operational services to an apartment for recurring billing. |
| **Preconditions** | Service Type exists. |
| **Postconditions** | Service linked to apartment. |
| **Normal Flow** | 1\. Staff opens Apartment details. 2\. Adds Service Subscription. 3\. Selects Service Type and Start Date. 4\. Saves. |
| **Alternative Flows** | **A1. Cancel Service**: Set End Date. |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Ad-hoc |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-20: Generate Invoice

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-20 Generate Invoice |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin, System |
| **Secondary Actors** | None |
| **Trigger** | End of billing cycle. |
| **Description** | Create monthly bills based on Rent and Service usage. |
| **Preconditions** | Meter readings entered. Prices set. |
| **Postconditions** | Invoices created in "Unpaid" status. |
| **Normal Flow** | 1\. Admin clicks "Generate Monthly Invoices". 2\. System iterates active contracts/services. 3\. System calculates totals. 4\. System creates Invoice & Details records. 5\. System sends notification to residents. |
| **Alternative Flows** | None |
| **Exceptions** | **E1. Missing Reading**: Alert admin for missing data. |
| **Priority** | High |
| **Frequency of Use** | Monthly |
| **Business Rules** | Invoice Date \= Generation Date. Due Date \= \+X days. |
| **Assumptions** | None |

## UC-21: View Invoices

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-21 View Invoices |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Resident |
| **Secondary Actors** | None |
| **Trigger** | Resident receives bill notification. |
| **Description** | Residents check their payment obligations and details. |
| **Preconditions** | Invoice generated. |
| **Postconditions** | Invoice displayed. |
| **Normal Flow** | 1\. Resident goes to "My Bills". 2\. System lists unpaid and paid invoices. 3\. Resident views detail of a bill. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | High |
| **Frequency of Use** | Monthly |
| **Business Rules** | Only own bills visible. |
| **Assumptions** | None |

## UC-22: Process Payment

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-22 Process Payment |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | None |
| **Trigger** | Resident pays via cash or transfer confirmation. |
| **Description** | Manually record payment for an invoice. |
| **Preconditions** | Invoice exists. |
| **Postconditions** | Invoice status "Paid". Transaction recorded. |
| **Normal Flow** | 1\. Staff finds Invoice. 2\. Clicks "Pay". 3\. Enters Amount and Method. 4\. System updates Invoice status and Balance. 5\. Generates receipt. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | High |
| **Frequency of Use** | Daily |
| **Business Rules** | Partial payment may be allowed (Status "Partial"). |
| **Assumptions** | Online payment handled by system automation (out of manual scope here). |

## UC-23: View Payment History

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-23 View Payment History |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Resident |
| **Secondary Actors** | None |
| **Trigger** | Resident checks past payments. |
| **Description** | View list of completed transactions. |
| **Preconditions** | Transactions exist. |
| **Postconditions** | History shown. |
| **Normal Flow** | 1\. Navigates to "Payment History". 2\. Views list. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Low |
| **Frequency of Use** | Rare |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-24: Submit Request

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-24 Submit Request |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Resident |
| **Secondary Actors** | None |
| **Trigger** | Issue in apartment (e.g., leak). |
| **Description** | Create a support ticket/request. |
| **Preconditions** | Logged in. |
| **Postconditions** | Request status "New". |
| **Normal Flow** | 1\. Resident clicks "New Request". 2\. Enters Title, Description, Type. 3\. Uploads photo (optional). 4\. Submits. 5\. System notifies staff. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Ad-hoc |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-25: Process Request

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-25 Process Request |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | None |
| **Trigger** | New request received. |
| **Description** | Manage lifecycle of a request (Assign \-\> Work \-\> Resolve). |
| **Preconditions** | Request exists. |
| **Postconditions** | Request resolved. |
| **Normal Flow** | 1\. Staff views pending requests. 2\. Opens one. 3\. Updates status to "In Progress". 4\. Assigns technician. 5\. Upon completion, sets "Resolved". |
| **Alternative Flows** | **A1. Reject**: Mark "Closed" if invalid. |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Daily |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-26: Post Announcement

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-26 Post Announcement |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Admin |
| **Secondary Actors** | None |
| **Trigger** | Need to broadcast info. |
| **Description** | Publish news to resident dashboard. |
| **Preconditions** | Admin access. |
| **Postconditions** | Announcement visible. |
| **Normal Flow** | 1\. Admin creates Announcement. 2\. Enters Title, Content. 3\. Publishes. 4\. System notifies users. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Weekly |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-27: View Notifications

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-27 View Notifications |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | All Users |
| **Secondary Actors** | None |
| **Trigger** | Bell icon activity. |
| **Description** | See list of alerts. |
| **Preconditions** | Logged in. |
| **Postconditions** | Notifications marked read. |
| **Normal Flow** | 1\. User clicks Notification icon. 2\. Views list. 3\. Clicks item to go to source. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Daily |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-28: Send Message

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-28 Send Message |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Resident, Staff |
| **Secondary Actors** | None |
| **Trigger** | Need to chat. |
| **Description** | Send text message in conversation. |
| **Preconditions** | Conversation exists. |
| **Postconditions** | Message saved and sent. |
| **Normal Flow** | 1\. Open Chat. 2\. Type message. 3\. Send. 4\. System updates chat view. |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Low |
| **Frequency of Use** | Daily |
| **Business Rules** | None |
| **Assumptions** | None |

## UC-29: Book Amenity

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-29 Book Amenity |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Resident |
| **Secondary Actors** | None |
| **Trigger** | Resident wants to use facility. |
| **Description** | Reserve a time slot. |
| **Preconditions** | Amenity available. |
| **Postconditions** | Booking created. |
| **Normal Flow** | 1\. Resident selects Amenity. 2\. Checks calendar. 3\. Selects time slot. 4\. Confirms booking. 5\. System validates availability. 6\. Booked. |
| **Alternative Flows** | None |
| **Exceptions** | **E1. Conflict**: Slot taken. |
| **Priority** | Low |
| **Frequency of Use** | Weekly |
| **Business Rules** | Limits on hours per resident (optional). |
| **Assumptions** | None |

## UC-30: Manage Parcels

| Field | Description |
| :---- | :---- |
| **UC ID and Name** | UC-30 Manage Parcels |
| **Created by** | Group 5 |
| **Date created** | 2026-02-06 |
| **Primary Actor** | Staff |
| **Secondary Actors** | Resident |
| **Trigger** | Courier delivers package. |
| **Description** | Log incoming parcel and notify resident. |
| **Preconditions** | Resident known. |
| **Postconditions** | Parcel logged. |
| **Normal Flow** | 1\. Staff receives package. 2\. Enters Apt Number or Resident. 3\. Logs Tracking Number. 4\. System notifies resident. 5\. \*\*(Pickup)\*\* Resident comes, staff marks "Picked Up". |
| **Alternative Flows** | None |
| **Exceptions** | None |
| **Priority** | Medium |
| **Frequency of Use** | Daily |
| **Business Rules** | None |
| **Assumptions** | Storage space available. |

# **III. Functional Requirements**

## **1\. System Functional Overview**

### 1.1 Screens Flow

| Actor | Screen/Feature | User story |
| :---- | :---- | :---- |
| All Users | Login | As a user, I want to log in so I can access the system features based on my role. |
| All Users | Forgot Password | As a user, I want to recover my password so I can regain system access if I forget credentials. |
| All Users | Dashboard Home | As a user, I want a dashboard so I can quickly see tasks, summaries, and important notifications. |
| All Users | User Profile | As a user, I want to update my profile so my contact information stays current. |
| All Users | Change Password | As a user, I want to change my password so my account remains secure. |
| All Users | Notification Center | As a user, I want to view notifications so I don't miss announcements, invoices, or request updates. |
| Admin | User Management List | As an admin, I want to view all accounts so I can manage users and their access. |
| Admin | Create/Edit User | As an admin, I want to create or edit user accounts so I can onboard staff/managers/admins and maintain data. |
| Admin | Block/Unblock User | As an admin, I want to block/unblock accounts so I can prevent unauthorized access when needed. |
| Admin | Role Management | As an admin, I want to manage roles/permissions so access control matches organization policies. |
| Manager | Apartment List (Manager View) | As a manager, I want to view all apartments so I can monitor occupancy and unit status. |
| Manager | Contract List | As a manager, I want to view contracts so I can track lease status and renewals. |
| Manager | New Contract Form | As a manager, I want to create a contract so I can onboard residents and formalize agreements. |
| Manager | Contract Details | As a manager, I want to view contract details so I can verify terms and member information. |
| Manager | Service Types List | As a manager, I want to manage service types so I can define billable services provided by the building. |
| Manager | Service Price Config | As a manager, I want to configure service prices so monthly invoices are calculated correctly. |
| Manager | Amenity Configuration | As a manager, I want to manage amenities so residents can book facilities correctly and rules are enforced. |
| Manager | Announcement Manager | As a manager, I want to manage announcements so I can broadcast notices to residents and staff. |
| Manager | Post Announcement | As a manager, I want to post an announcement so residents receive important building updates. |
| Manager | Invoice Generation | As a manager, I want to generate invoices so residents can be billed monthly based on usage and services. |
| Manager | Request Queue (Manager View) | As a manager, I want to review requests so I can oversee escalations and ensure service quality. |
| Staff | Apartment List (Staff View) | As staff, I want to view apartments so I can support operations such as check-in, service registration, and inquiries. |
| Staff | Apartment Details | As staff, I want to view apartment details so I can see residents, services, and unit status. |
| Staff | Register Service | As staff, I want to register services for an apartment so residents are subscribed to the correct fixed services. |
| Staff | Resident List | As staff, I want to view residents so I can assist with daily operations and data updates when necessary. |
| Staff | Resident Profile | As staff, I want to view resident profiles so I can manage cards, vehicles, and basic resident info. |
| Staff | Issue Resident Card | As staff, I want to issue resident cards so residents can access the building securely. |
| Staff | Lock/Extend Card | As staff, I want to lock or extend cards so access is controlled and cards stay valid when appropriate. |
| Staff | Register Vehicle | As staff, I want to register vehicles so parking can be managed and security checks are consistent. |
| Staff | Visitor Log | As staff, I want a visitor log so I can track guests entering and leaving the building. |
| Staff | Visitor Check-In/Out | As staff, I want to check visitors in/out so security records are accurate and real-time. |
| Staff | Meter Readings Input | As staff, I want to input meter readings so usage-based services can be billed accurately. |
| Staff | Request Queue (Staff View) | As staff, I want to view assigned requests so I can process and resolve them efficiently. |
| Staff | Process Request Detail | As staff, I want to update request details so residents receive progress updates and issues get resolved. |
| Staff | Parcel Management | As staff, I want to manage parcels so deliveries are recorded and residents can be notified. |
| Staff | Log Incoming Parcel | As staff, I want to log parcels so the building has an auditable delivery trail. |
| Staff | Invoice Processing | As staff, I want to process invoices at the desk so I can support resident payments and inquiries. |
| Staff | Record Cash Payment | As staff, I want to record payments so the system reflects accurate payment status and history. |
| Resident | My Invoices | As a resident, I want to view invoices so I know what I need to pay each month. |
| Resident | Invoice Detail | As a resident, I want to view invoice details so I can understand charges and consumption. |
| Resident | Payment History | As a resident, I want to view payment history so I can track past payments and receipts. |
| Resident | My Requests | As a resident, I want to view my requests so I can track progress and outcomes. |
| Resident | Submit New Request | As a resident, I want to submit a request so I can report issues or ask for support. |
| Resident | Amenity Booking | As a resident, I want to see available amenities so I can plan my facility usage. |
| Resident | Book Time Slot | As a resident, I want to book an amenity time slot so I can use facilities without conflict. |
| Resident | Pre-register Visitor | As a resident, I want to pre-register visitors so guests can enter smoothly with proper authorization. |
| Resident | My Contract Info | As a resident, I want to view contract information so I understand my lease terms and obligations. |
| Resident | Messages / Chat | As a resident, I want to send messages so I can communicate with staff regarding support and issues. |
| Staff | Messages / Chat | As staff, I want to reply to messages so I can assist residents and coordinate operations. |

 

### 1.2 Screen Descriptions (Feature/ Screen/Description)

### 

| Actor | Screen/Feature |  User story |
| ----- | ----- | ----- |
| **All Users** | Home Page | As a User, I want to access the home page so that I can quickly navigate to the relevant sections of the site. |
| Candidate | Register Account Page | As a Candidate, I want to register an account so that I can apply for jobs and manage my profile |
| Candidate | Login Account Page | As a Candidate, I want to log into the system so that I can access job listings and my application history. |
| Candidate | Saved Jobs Page | As a Candidate, I want to view a list of saved jobs so that I can revisit and apply for them later. |
| Candidate | Advanced Job Search Page | As a Candidate, I want to perform an advanced job search so that I can find jobs that best match my criteria. |
| Candidate | Create/Edit CV Page | As a Candidate, I want to create and edit my CV so that I can present my qualifications to recruiters. |
| Candidate | Candidate Profile Page | As a Candidate, I want to view and update my profile so that my personal and professional information stays current. |
| Candidate | Job Details Page | As a Candidate, I want to view job details so that I can understand the requirements before applying. |
| Candidate | Applied Jobs List | As a Candidate, I want to view the list of jobs I have applied to so that I can track my application status. |
| Candidate | Apply for a Job | As a Candidate, I want to apply for a job so that I can be considered for the position. |
| Candidate | Interview Schedule Page | As a Candidate, I want to view my interview schedule so that I can prepare and attend on time. |
| Candidate | Enroll for Online Test | As a Candidate, I want to enroll in an online test so that I can demonstrate my skills to recruiters. |
| Candidate | Online Test Result | As a Candidate, I want to view my test results so that I can understand my performance and readiness. |
| Candidate | Online Test History Page | As a Candidate, I want to access my test history so that I can track my progress and test attempts. |
| Candidate | Candidate Dashboard | As a Candidate, I want to see a summary of my activities on a dashboard so that I can easily manage job applications and tests. |
| Candidate | Video Call Function | As a Candidate, I want to join a video call with recruiters so that I can attend interviews online. |
| Recruiter | List of Job Applicants | As a Recruiter, I want to view the list of applicants for a job posting so that I can review and shortlist candidates. |
| Recruiter | Job Posting Page | As a Recruiter, I want to create and post job openings so that candidates can apply to them. |
| Recruiter | List of Posted Jobs | As a Recruiter, I want to manage the list of jobs I have posted so that I can edit or remove them if necessary. |
| Recruiter | Recruiter Profile Page | As a Recruiter, I want to manage my company profile so that candidates see accurate information. |
| Recruiter | Register Account Page | As a Recruiter, I want to register an account so that I can start posting jobs and reviewing candidates. |
| Recruiter | Login Account Page | As a Recruiter, I want to log into the system so that I can manage my job postings and review applications. |
| Recruiter | Job Details Page | As a Recruiter, I want to view job details as published so that I can verify how candidates see them. |
| Recruiter | Create/Edit Interview | As a Recruiter, I want to create or edit interview schedules so that I can arrange meetings with selected candidates. |
| Recruiter | Interview Schedule Page | As a Recruiter, I want to view and manage upcoming interviews so that I can stay organized and prepared. |
| Recruiter | Create Online Test | As a Recruiter, I want to create online tests so that I can evaluate candidate abilities relevant to the job.. |
| Recruiter | Edit Online Test | As a Recruiter, I want to edit existing online tests so that I can correct or update the content. |
| Recruiter | Service Package Management | As a Recruiter, I want to manage my service packages and wallet so that I can keep track of purchased features. |
| Recruiter | View All Service Package | As a Recruiter, I want to view available service packages so that I can choose the most suitable plan. |
| Recruiter | Buy Service Package | As a Recruiter, I want to purchase a service package so that I can access premium features. |
| Recruiter | Statistic Manager | As a Recruiter, I want to view recruitment statistics so that I can measure performance and improve outcomes. |
| Recruiter | Recruiter Dashboard | As a Recruiter, I want to access a personalized dashboard so that I can monitor all recruitment activities. |
| Recruiter | Auto Import Question for Test | As a Recruiter, I want to automatically import questions for tests so that I can save time during test creation. |
| Recruiter | CV Search/Filter | As a Recruiter, I want to search and filter candidate CVs so that I can find the best-suited profiles. |
| Recruiter | View System Promotion/Voucher | As a Recruiter, I want to view system-wide promotions or vouchers so that I can apply discounts when purchasing services. |
| Recruiter | Video Call Function | As a Recruiter, I want to initiate a video call with a candidate so that I can conduct online interviews. |
|  |  |  |

### 1.3 Screen Authorization

| Screen / Function | Candidate | Recruiter | Admin | Moderator | All |
| ----- | ----- | ----- | ----- | ----- | ----- |
| Home Page | ✓ | ✓ | ✓ | ✓ | ✓ |
| Google Authentication | ✓ | ✓ |  |  |  |
| Register Account Page | ✓ | ✓ |  |  |  |
| Login Account Page | ✓ | ✓ | ✓ | ✓ |  |
| Candidate Profile Page | ✓ |  |  |  |  |
| Saved Jobs Page | ✓ |  |  |  |  |
| Advanced Job Search Page | ✓ |  |  |  |  |
| Create/Edit CV Page | ✓ |  |  |  |  |
| Job Details Page (View Job Post) | ✓ | ✓ | ✓ | ✓ | ✓ |
| Applied Jobs List | ✓ |  |  |  |  |
| Apply for a Job | ✓ |  |  |  |  |
| List of Job Applicants |  | ✓ |  |  |  |
| Job Posting Page |  | ✓ |  |  |  |
| List of Posted Jobs | ✓ | ✓ | ✓ | ✓ | ✓ |
| Recruiter Profile Page |  | ✓ | ✓ | ✓ |  |
| Manage User Accounts |  |  | ✓ |  |  |
| View Promotions List |  |  | ✓ | ✓ |  |
| Create Promotion |  |  | ✓ | ✓ |  |
| Admin Role Management |  |  | ✓ |  |  |
| View Candidate Detailed Application |  | ✓ |  |  |  |
| Create/Edit Interview |  | ✓ |  |  |  |
| Interview Schedule Page | ✓ | ✓ |  |  |  |
| Online Test Management |  | ✓ |  |  |  |
| Create Online Test |  | ✓ |  |  |  |
| Edit Online Test |  | ✓ |  |  |  |
| Enroll for Online Test | ✓ |  |  |  |  |
| Online Test Result | ✓ |  |  |  |  |
| Online Test History Page | ✓ |  |  |  |  |
| Service Package Management (Wallet) |  | ✓ |  |  |  |
| Promoting Job Post Management |  | ✓ |  |  |  |
| View Promoted Job Post | ✓ |  |  |  |  |
| Manage Pending Promoted Job Posts |  |  | ✓ | ✓ |  |
| View/Filter All Job Posts |  |  | ✓ | ✓ |  |
| Manage Pending Job Posts |  |  | ✓ | ✓ |  |
| Approve Recruiter’s Account |  |  | ✓ | ✓ |  |
| Manage Service Packages |  |  | ✓ | ✓ |  |
| Blog Post |  |  | ✓ | ✓ |  |
| View Blog/News | ✓ | ✓ | ✓ | ✓ | ✓ |
| View All Service Packages |  | ✓ |  |  |  |
| Buy Service Package |  | ✓ |  |  |  |
| Online Payment by VNPay API |  | ✓ |  |  |  |
| Statistic Manager |  | ✓ | ✓ | ✓ |  |
| Recruiter Dashboard |  | ✓ |  |  |  |
| Candidate Dashboard | ✓ |  |  |  |  |
| Notification System |  |  | ✓ | ✓ |  |
| Auto Import Question for Test |  | ✓ |  |  |  |
| CV Search/Filter |  | ✓ |  |  |  |
| Review Pending Test |  |  | ✓ | ✓ |  |
| Recruiter Revenue Report |  |  | ✓ | ✓ |  |
| Loyal Recruiter Report |  |  | ✓ |  |  |
| View System Promotion/Voucher |  | ✓ |  |  |  |
| Service Statistic Manager |  |  | ✓ |  |  |
| Recruiter Report Download |  |  | ✓ | ✓ |  |
| Admin Dashboard |  |  | ✓ | ✓ |  |
| Video Call Function | ✓ | ✓ |  |  |  |
| Payment History Page |  |  | ✓ |  |  |
| Review Ban Request |  |  | ✓ |  |  |
| Create Ban Request |  |  |  | ✓ |  |
| Interview Reminder through Email | ✓ |  |  |  |  |

### 1.4 Non-Screen Functions

| \# | Feature | System Function | Description |
| ----- | ----- | ----- | ----- |
| 1 | Job Exparity Management  | Batch/Cron Job | Automatically deactivates job posts after their expiration date. |
| 2 | Notification Delivery | Batch/Cron Job | Sends scheduled emails or SMS alerts to users about job matches or updates. |
| 3 | Data Backup | Batch/Cron Job | Performs regular backups of the database to ensure data safety. |
| 4 | Account Maintenance | Batch/Cron Job | Flags or removes inactive accounts based on usage patterns. |
| 5 | Analytics Aggregation | Batch/Cron Job | Summarizes user activity for reporting and dashboard metrics. |
| 6 | Resume Parsing | Background Service | Extracts structured data from uploaded resumes for easier matching. |
| 7 | Candidate Matching | Background Service | Scores and ranks applicants based on job criteria and relevance. |
| 8 | Notification Handling | Background Service | Manages asynchronous delivery of alerts and messages. |
| 9 | Document Storage | Background Service | Handles secure upload and retrieval of user documents. |
| 10 | User Management | API | Supports registration, login, and profile updates |
| 11 | Job Posting | API | Allows recruiters to create, edit, and remove job listings. |
| 12 | Application Tracking | API | Enables job seekers to apply and monitor application status. |
| 13 | Search Functionality | API | Provides filtered and ranked search results for jobs and candidates. |
| 14 | Analytics Access | API | Delivers usage statistics and performance data to dashboards. |
| 15 | Third-Party Integration | API | Connects with external services like LinkedIn, Stripe, or resume parsers. |
| 16 | Data Integrity Enforcement | DB Trigger / Stored Proc. | Ensures business rules are applied at the database level. |
| 17 | Audit Logging | Logging & Monitoring | Tracks user actions for compliance and security. |
| 18 | Error Tracking | Logging & Monitoring | Captures system errors for diagnostics and debugging. |
| 19 | Performance Monitoring | Logging & Monitoring | Monitors system health and resource usage in real time. |

### 1.5 Database Diagram

#### *a. Diagram*

![][image3]

#### *b. Table Descriptions*

| No | Table | Description |
| :---- | :---- | :---- |
| **1** | Apartments | Information about apartments including number, floor, area, and status. |
| **2** | Residents | Profiles of residents, linking to Users table, including personal details and residency status. |
| **3** | ResidentCards | Access cards issued to residents for building access, elevator, or parking. |
| **4** | Vehicles | Vehicles registered by residents for parking management. |
| **5** | ServiceTypes | Definition of available services (e.g., electricity, water) and their measurement units. |
| **6** | ServicePrices | Pricing history for services, allowing rate changes over time. |
| **7** | MeterReadings | Monthly usage readings for metered services (water, electricity). |
| **8** | Invoices | Billing records for apartments, including service fees and rent. |
| **9** | InvoiceDetails | Line items within an invoice detailing specific charges. |
| **10** | PaymentTransactions | Records of payments made against invoices. |
| **11** | ApartmentServices | Services registered for specific apartments (e.g., internet, cable TV subscriptions). |
| **12** | Requests | Maintenance, complaint, or service requests submitted by residents. |
| **13** | RequestAttachments | Files or images attached to requests for evidence or clarification. |
| **14** | Announcements | News and notices published by property management. |
| **15** | Documents | Official documents, handbooks, and regulations shared with residents. |
| **16** | Visitors | Log of visitors registered by residents for security access. |
| **17** | Parcels | Tracking of packages and deliveries received for residents. |
| **18** | Amenities | Common facilities available for booking (e.g., BBQ, Tennis, Pool). |
| **19** | AmenityBookings | Reservations of amenities by residents. |
| **20** | Users | Core user accounts for all system actors (Admin, Staff, Residents). |
| **21** | Notifications | Alerts and notifications sent to users. |
| **22** | Conversations | Chat groups or direct message threads. |
| **23** | ConversationParticipants | Users participating in specific conversations. |
| **24** | Messages | Individual messages sent within conversations. |
| **25** | MessageReadReceipts | Tracking who has read specific messages. |
| **26** | MessageReactions | Emoji reactions to messages. |
| **27** | Contracts | Lease or purchase agreements for apartments. |
| **28** | ContractMembers | Individuals listed on contracts (tenants, guarantors) linking to Residents. |

### 1.6 Package Diagram

![][image4]

# IV. Non-Functional Requirements

## 1\. External Interfaces

### 1.1 User Interfaces

* **Web Interface**: The system shall provide a responsive web interface compatible with modern browsers (Chrome, Firefox, Edge, Safari).  
* **Style Standards**: The UI shall follow a consistent theme using Bootstrap framework clearly distinguishing between different user roles (Admin dashboard vs Resident portal).  
* **Usability**: Frequently used functions (e.g., Request submission, Bill payment) shall be accessible within 3 clicks.

### 1.2 Hardware Interfaces

* **Server**: Requires a standard Windows/Linux server capable of hosting .NET 8 runtime and SQL Server.  
* **Client**: Accessible on desktops, laptops, and mobile devices with internet connectivity.

### 1.3 Software Interfaces

* **Operating System**: Windows Server for deployment (preferred) or Linux.  
* **Database**: Microsoft SQL Server (2019 or later).  
* **Frameworks & Libraries**:  
  * ASP.NET Core 8.0 (Razor Pages)  
  * Entity Framework Core 8.0  
  * jQuery & Bootstrap 5  
* **External Services**:  
  * SMTP Server for email notifications.  
  * Integration APIs for localized payment gateways (e.g., VNPay \- Simulated/Mocked).

### 1.4 Communications Interfaces

* **Protocol**: HTTP/HTTPS over TCP/IP.  
* **Data Format**: JSON for AJAX requests and API interactions.

## 2\. Quality Attributes

### 2.1 Performance

* **Response Time**: 90% of page loads should complete within 2 seconds.  
* **Concurrency**: Support at least 50 concurrent users without degradation.  
* **Throughput**: Handle transaction processing (e.g., bill payments) with correct data consistency.

### 2.2 Security

* **Authentication**: Secure login using Identity or custom cookie authentication with hashed passwords.  
* **Authorization**: Strict Role-Based Access Control (RBAC) ensuring Residents cannot access Admin features.  
* **Data Protection**: Sensitive data (passwords, personal info) is encrypted or handled securely.  
* **Input Validation**: Protection against SQL Injection, XSS, and CSRF attacks.

### 2.3 Reliability

* **Availability**: System should be available 24/7 with minimal downtime for maintenance.  
* **Data Integrity**: Database transactions ensure data consistency (ACID properties), especially for financial records (Invoices/Payments).  
* **Error Handling**: Graceful error messages for users instead of technical stack traces.

### 2.4 Maintainability

* **Code Quality**: Follows C\# coding standards and Repository Pattern as defined in project guidelines.  
* **Modularity**: Separation of concerns (Data, Logic, UI) to facilitate easy updates.

### 2.5 Usability

* **Learnability**: New residents should be able to navigate basic features (View Bill, Send Request) without training.  
* **Accessibility**: Basic support for standard accessibility guidelines.

### 2.3 Descriptions (Cai nay sau cho len 4.3 nha) {#2.3-descriptions-(cai-nay-sau-cho-len-4.3-nha)}

*\[This part describes the use cases, you can follow the table form as below\]*

| ID | Use Case | Actor(s) | Use Case Description |
| ----- | :---- | :---- | :---- |
| 01 | Register Recruiter Account | New User want to recruit | As a employee of Human Resource Department, I want register account with verified information of my company to find new employees |
| 02 | Register User Account | Guest (New User) | As a new user, I want to register an account with my full name, email and password so that I can access the features of the system |
| 03 | Login System | Registered User | As a user with a specific role, I want to be redirected to the appropriate page after login so that I can immediately access the privileged features relevant to my role(Recruiter/Candidate) |
| 04 | Reset Password | Registered User | Reset Password: As a registered user, I want to request a password reset by entering my email so that I can regain access to my account if I forget my password |
| 05 | View Recruitment posts | Candidate, Guests | As a job-seeker, I expect to access recruitment posts from several fields, factories and companies that I can choose and apply the most proper job. |
| 06 | Create recruitment post | Recruiter | As a registered recruiter, I want to create job posts with several information fields and several boost for my posts. |
| 07 | Create promotion  | Admin, Moderator | As an admin of the system, I want to create several promotion packages that I can encourage the purchasement of service package posts. |
| 08 | Create Service Package (For Job Posts) | Admin, Moderator | As an admin, I want to create several promotion packages, so that I can encourage the purchase of job posting service packages. |
| 09 | View Revenue Statistics | Admin  | As an admin, I want to view revenue statistics, so that I can analyze financial performance and service popularity. |
| 10 | Solve Reported Job Posts | Admin, Moderator | As an admin, I want to view and take action on reported job posts, so that I can maintain the quality and trustworthiness of job listings. |
| 11 | View Partner Companies | Admin, Moderator | As an admin, I want to view a list of partnered or active companies, so that I can monitor platform engagement from recruiters. |
| 12 | Moderate Inappropriate Job Posts | Admin, Moderator | As a moderator, I want to report inappropriate job posts, so that the admin can review and maintain platform standard |
| 13 | Manage User accounts | Admin  | As an admin, I want to view all user account and some contact information, and I also can ban, delete some inappropriate users. |
| 14 | Apply for Jobs | Candidate | As a candidate, I want to browse and apply for job posts, so that I can find suitable employment opportunities. |
| 15 | Take Entry Test | Candidate | As a candidate, I want to take assigned entry tests, so that I can demonstrate my skills to recruiters. |
| 16 | View Entry Test Results | Candidate | As a candidate, I want to view my entry test results, so that I can understand my performance and next steps. |
| 17 | Create CV with Templates | Candidate | As a candidate, I want to choose from different CV templates and create my resume, so that I can present myself professionally. |
| 18 | Buy Service Package | Recruiter | As a recruiter, I want to browse and purchase service packages, so that I can post jobs on the platform. |
| 19 | Create Online Entry Test | Recruiter | As a recruiter, I want to create and assign online entry tests, so that I can assess candidate skills before interviews. |
| 20 | Set Interview Schedule | Recruiter | As a recruiter, I want to schedule interviews with applicants, so that I can plan the recruitment process efficiently. |
|  |  |  |  |
|  |  |  |  |
|  |  |  |  |
|  |  |  |  |
|  |  |  |  |

[image1]: <data:image/png;base64,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>

[image2]: <data:image/png;base64,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AIGAA1N6TUDADYZHwSSy4heI9V5MLwEXv7CsMNlatMUCzmIwfkHYcWPRF78TA1NzySWXpF4AA3HmmWemXjCmIODAlF4TEHCD4Rd1LeQkpHTsr8bsp2OJLQs4x5N4kwDTiJ1fQo7FmG1k75edFa60tB+nA/3x3HPPpV4AA7HZZpulXjCmIODAlF4TEHAwTjz99NOpV0+WLVsWGuNjjz02DRoa+kJvVKxcubJ4+OGHU++A3/959NFHJ/i/+eabxYknnji2ndLnPve51KsrW2+9deoFY8q43iuwJqXXBAQcjBOXX3556tWTww47rPj4xz9eWoOskVh/BDMKPBLcDf3GtddeO/zmmFtuuSX4l1UGdSYeRZ+K008/PfWaNhpV73WdNArvCXSnIh7t74XO46lCho2fSozbx1bjeK9Ad0qvCQg4GCfmzZuXevVEDbFm2Z87d25x5513dvzVIUnY6GMI2Rx++OFRrN9PErzTTjuFsP333z+MYgl/2LLrrrsW22yzTWfUS86P0Z955pliww03LDbZZJPi1VdfLc4999ywL/faa691znHXXXcV66yzToh7zjnndPydhvwUdsoppwR/P2q3S5GfH//H7LDDDmEEMvbXHHrKo/y23HLLMLInXC777bdfCNtll106v12oXHz+uFzEZZddFspkxowZxeOPPz5B2CpM+VC8G264oePvudOcvyuvvLJ49tlnQxpK6/rrr+/Yqiydhq7bG2+8Efz9aoPKSfFUzq+88koIc179asNkTCaSctF1sHCMJ8v1KxTyU74s8uzn92MV5ut94403hn37C8XTvsWgwvwubfwqh+Z3i6+D03d8i744beXTH14pfed5nHBZAJReExBwME7MmjUr9erKbbfd1hmN0mPFbbfdthOmTkmNtESB+Na3vhVEmfjsZz9bnHrqqR2x9eUvf3lCZ6x4DotH4Nx57rzzzuFY55ftz3/+83C8ZMmSYvfddw/7EknxIurnnXde8ZGPfCTsOw19UCMOPPDA4oorrgj7ykevkR2dS+ESqxJQQgJLv9uPksVPfvKTIHLME0880QlzuVgcSUA5zy4XE5fLN77xjeKDH/xg8eKLL4Zj/TaXy2mnnVb85V/+ZUdU6ffIXshG5/NSVto/5JBDwv4111wTjlevXh2Wu9K+05DIS8WMz/3UU0+FEUeHOY9T4bSHgc5rQRgL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>

[image3]: <data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnAAAALtCAYAAACo61k+AACAAElEQVR4Xuy935Pk1pXnV7+6NTPv/hP2yWH7ZZktkdR4IxyOGO2DHbbfHMvuItms6m5S1qMn1huxESOSRY7DISCr/bAPEimNJP4o0i8kgOpdx2yEpRmxSXbxZTWqAmqfmhEj9o+q1khkiF1VeXzPvbhZqJPIBJAFIAHk99NxGsDFj7x5E4n81AVwsHD16lWa9wAAAAAAaBMLLDCPHj2a24DAAQAAAKBtQOAgcAAAAABoGUbgdjaot7BAC70NLTVbq2p8oUcbO6PCc95YVa/Dw52NotvfGq5bZkDgAAAAANA2zgicFqStVS1yVuBYtBYWVo3U9cy8R492aKO3QDtnZGiHFla3hustqG31NnbE+hv6NXielUWev/XISKPZntm2rscZYTsVOF3XLX4N81pczq+ll4/fS3odRwMCBwAAAIC2cUbgWKZWV60MGTkyvXE2jFSxqFnBMtIWz+dxJWYsTbw93o5cfyiJWrLObn/LbnPB1IWXnyxwq1r+WNSsPJ7pqUvUUYobBA4AAAAAbeWMwOletFjATk+hmp4sI0LcM8ditjUssz1fvDyLEsuTLo/nnV0/eRrUyp5d30pbj3rcA5eUu1jA9LQatwIn558VuNPXNfUYlTcIHAAAAADaSGU3MVR1DV3ZAYEDAAAAQNuoTODaEhA4AAAAALSNhTfffHNEatoW03L9+nXi9w8AAAAA0Ca0wHUFKXZZAYEDAAAAQBuBwHXo/QMAAABgPjgjcCw144h8nyL1T43w/2eGztqTtOaruWrc14s4eqhn83zHOd1QhUhBywoIHAAAAADaSO4eOCtwvpIzHrKUOUrSfDXk4HFdpkb0NBeQKeN16kAKmg1OJSLLIHAAAAAAaCu5Ba4NSEGz8gaBAwAAAECXmBuBS5M4CBwAAAAA2kinBS4pbxA4AEAZrK2t6fyR8x4AgNnSWYGT8pYmcRA4AEBRWOAODg5G/iCcp+A2qAIpifMYR0dHslkASKWzApcnIHAAgKJA4KoTuIcPH4681jzFtWvXIHAgNxA49f6TPXQAADCJswL3Hi3qY8cVOkw5xjQtnltZpgcHhyPlRQMCV01A4EARcgtc5K/pXG/OGqcIMTngHJP0zeSCc8zQjxK54ni9FuSBk6dZAQDt58c//rEsKoWkwL333DJ9enCHXvvmEj1UYrS8tEhPvPIpfXNpiXpqaKY/oW8u8/QnarqnRO89ek5NHxw+op3XvkmvfHJAhzuv0ZV3HqYsv6jL+bV2XvsWvfPwgJYuvUKfPLSvZZbh4cryIn2qllleukJbz63QslpuednM+1a8vVOB+4yWeryd6XoSKxW4z17XUvzEyx/To/eepwuq7g8PR+vw/IVlWrr8VqLsM1q+8hbdf5hPUJ+/sCLWn31A4EARCgicb3K+8dB3tLyxv3GZFjdnLZ4X54GLM/k6qoyGOlct8suQFTiFCkB3qV7gjIjZP/xYrljQXvn0QJcbgetpQePpS0qglha5p25HCd+yFr5Rgbt0ZnmWMd4uH69YFs1rmTKzrJE+Xpcl7d1nlbgt2j9GryiBM8uwuPH2kj1wK3Hdp+mRq1rgrly+TEvq/R0kBO695y/o+n78QLUXy1r8Pu9/+ho9qd7XO8+Z+QtPvEzfUtNX3rpPn73+pBbbBSVqvByL4csfP1DyLNa/wJ+HapMlU3abX+P95+ny5Z6qxyK9fPuBrgvPuzel9OYJCBwoQm6By0MURUNxmwXyy5AVEDgAukvVAse9bkbI+Hiyo3u0ljIEbihhV96Jj0PxKdgnnkgVOF5+iUVDLf/wYCcxzj1wowJ3cLhDr3/LvsbbSl7OCtx7z63o8gcHd8y21HK8LXlszIrKBe7t+0ra3qNLql0WFxeVmF5Q7/OyETdVf+5lG/bAqeVZ4B6ylMU9cDzNAreyfIXeuh+flo0FTMucXj/ugeP1LxgB5GUPbE/eO88ZcTt8ny69fJsevAuBA82iVIGbNfLLkBUQOAC6S9UCJ48n8xT1CNwj3fvG0vTpa9yT1lMypaTu5Y91r2E+gVuky2rIp1dff3KFPn7wrl4nTeD4Nc4KmxA49ZrvP3+R3rrHkjfaJmUEBA4UAQLXofcPADgFAlddVCpwKa83LwGBA0XQAid3oraFRZZnBQQOgO5SlcDxdg8Pi592bFKch5deeoneeOMNWVwKEDgIHMgPeuA69P4BAKdULXBdQR4XJwW/9xs3blQmcF3o2TwP3K4nJyeyGIBUIHAdev8AgFMgcPmQx8VJUYfAMfxav//978Xc5sP1tnz++ed09+7d3PGTn/wEAgcKkVvgOI0I31/qJ/K+RXE6kTpzvU0ieaAxt9GPRnIZCBwA3aUugbPHkzSiIKBoMJDFpbC/v6+3HQQRDc7xGlLSJkVdAvf11wP6d//uf6Pj43adTkzuByxwsv0mBQQOFCW3wJGWNc7zpmTN53xwJt+bTuir88M1S+A4JskbBwQOgO5Sl8Ax3/nOd85MW66pY+Ma/7E78Ml1Iz3kYygLF0fgurrcd8xwWOaH5KlhEAwodPvkeY4uC0889QM/oP1+n0Ilh67aVhUCt7KyQg8ePBgpr0vgvvyS6Be/+J/U+0oXuH313gf7AQXR2fcdhT71g5AiJUFpLcLlSViCVWvS9nY56a+4jSwQOFA1+QWuBcgvBAcEDoD5pE6Be/XV/+rMtMGIVRS4FCYFTsmakTi1hJK0dUfJWixwvhcqKXOJvcRPChzLmyobETgtgKEumxZ5XEweN2V5fQJ3TH/7t3+l2uhYLGHYD/p0vOcqWTuhft8lb2+Prn24S/1rT9O6GmqR65vYOwn1vGPVRlzu7Z2otuuTr4fben3H25MvMRXcRhYIHKiauRE4Wc4BgQOgu9QpcG1GHhe5980eN2UvXF0C99VXX9G///d/N0HgAor6f07XvCMlvZ6W2L4SMv20ICVArhp6qvxD7rkMT/Q8dlwu52U9p6/ELaQ9LzC9m259Asfteu/evZFyCBwoSucFzn5hZBkHBA6A7gKBy0fymJiUtzSJq1fgfjFW4JoKt5ElTeAuXrw4tlMBAgeKogVO7khtC4sszwoIXHu4evXqyOc3T/HCCy+c6zqneQQCl4/kfiblTcpGXQL3u9/9jv7xH79Q+3y7ZIbbyJImcMk2lb1wEDhQlLnogRsXELj2AIGDwBUFApcPua9NijoETr5m28IiBS7Z+5YmcRA4UBQIXIfef5c5Fbit4cHPPES82lhcfVf9WKvh4gL1Xr0zMr+ugMAVBwKXD7mvTYo6BK7NcBtZpMBlBQQOFCW3wJk8cJHOAadvuE4MnbUnaS3OB+frRRw9ZDjdCN+NpcZMupGI047E65YIy9g0kff9g9liBW5xYfXMQW9rddEIXW/DzF/s0epqT5X1aOPOHV1+hwVMD3fU+gu0+u4hHW49G4vgqpYzLYRquUuLZnuvqnU3LsXbVhLH5bze1rP29V6lR2ob/HpmOz0tlMltyQP0eQICV5wmCJxJ8UHkrj9F696evgsybNjnKPe1SQGBmwy3kQUCB6omt8C1AfmFyBNdev9dZihwsahx7GxcUkLWI+6VW1WCZQWqt3FHCZcZsvC9e7ilx++o5d9VP7yLSr7ubK3qHrXDRztm+oxw7cQ9b1ta3qzYscCxsPHrPcuvpwVOydrOhp5/yIKot1X+czIhcMVpksDpuyPVCP8R6/ihLmsKfAwsEhC48fB31QKBA1WjBW7eAzQfK3AsZvbUabrAcc/boZ5maeNetF6vpwWNe89Y4PQBU8mX7omLt2XmWQGz4pZH4LjnLZ6OJfDM65QUELjiNEHgms6LL744clYiT8xa4HQiXxVB6NNJGOh8b1HQpz0/0L2cnNftPLnxpoW/qxYIHKiaBVlQN/Z6JgAmMbwGbmv1zDVww1OoSrR4vhQ4I3lW+swpVC1lajtG4OKyeBlzCrQ3FDe7/MgpVH69EYFLbAunUGcOBC6b27dvj+xrWcHvvaq2LSpw112X/H6ffM8hz+3rR5f1fV8/WWHWAsdCJjsLsgICB4owc3OCwIE84C5UCFxRqpKMLgkcIx+qPilwDdxk+LvKsBjzPiK/x1nB7QuBA3mZuTlB4EAeIHAQuKJULXDyM2pTJJHzJgUEbjKybYuCHjhQhJmbEwQO5IFPL6QdHNPKmoqtq+zVyAp7egUCV4yqBa4rSEmbFHUInHzNtoUF18CBqoE5gVbQJYGTB+6sgMBNBwQuH3J/mxR1CFyb4TayQOBA1UDgQCuAwEHgitIEgfOjAQ0o0rkv839+EUW5lz0/cn+bFBC4yXAbWSBwoGogcKAVJAXuzTf/kr74wuy6yQMmw/m2hgmmeZyTSnMOLp1Qmst8XeaoaQ6TkDoif/0p4p9MTkadXE8nsOYym5n6HNi6ygN3VkDgpqMJAhcNInJdX48HnPZCRTTwVZna6/gOSvWZ+np/DFW5R6FjyoIBi189yP1tUjRD4PYp6Pd1qhBOG8LtxNN1tdckuI0sEDhQNRA40ApkD9xf/MVf6GGyjGFBY3w/Hmqh84cyxj+WVvLMQ0KUnEUDilSZXl+sx+twmRHA82HrKg/cNhYXF0fKOCBw09EMgVOfWeDq/cdXcuY43Lt2KnBuyALH8sbLGoHjMghcFvv6+7Af9Gn9w1360O3rXHB1tdk4uI0sEDhQNRA40AqkwFnSypqKras8cHOwvJn8caN3N0LgpqMJAtcG5P42KZomcNzz5u6dkKeGnr8HgQNzBQQOtIJ5Ebg0iYPATQcELh9yX5wUzRG4ZsJtZIHAgaqBwIFWYAWurcEkh8mw4paM5HwI3HRULXDyc2xTJJHzJgUEbjLJtoXAgaqBwIFW0OUeOClvshcOAjcdVQtcV5D746SAwE2G28gCgQNVA4EDraDLApcVELjpgMDlQ+5vkwICNxluIwsEDlQNBA60AggcBK4oTRA4frh6VTndgr5LruPQyTm3L/e3SQGBmwy3kQUCB6oGAgdaQX6B43QNjijLj0wWolOPUJxy5JzYusoDd1ZA4KajCQIXXHPI4VQhnIswHg6igFzXMXkHleD5QaT2M1fP55xxYahCfdZR4JLn+eT6IfW5/GRAAa832FfbfVqJW0RBYGIwCMiLt83LFtlT5P42KSBwk+E2skDgQNVA4EAryCtwVeSBiyJOtJpYlrenptf5R9k3P8R5sHW1QlY0IHDFmL3AsVgZEQtt7rcB74ucF46fzEA6F9y6Eyr5cvV83wuVsLk6L5yv5gXBgEKXU2SEahukJM5TP/AD2u/3lcDta3lTE3TN9dS6nhI+3naYe59kpEhMiqYI3D4nRVZtELLs7Ctp7as2Dvrk7+2Rf+3bdBxymfrubve19PrXv03XPtwl2t6mKvUoeTyCwIGqgcCBVpBX4EhntTfCxj+SVsSUoakfyihOqMplZrimwuEHMliBE+tpgVPl6+pHkefpZa3AsQwW6JrjuvKP3/Xr1wsHBK44sxe4mlDi0g9PjMxMgRSJSdEkgev3Ay2z/HQL/m6w8Pb7Ie05SuSU3PmeQ57LkufpsoDbBwIHOgQEDrSC3ALXYM5TVwhcceZG4M6JFIlJ0RyB6+sezKJsc8+lLCyR5HccAgeqBgIHWkGXBO7u3buFAqdQp6NqgZM/wG2KJHLepGiKwDWVZNtC4EDVzFTgON+VqYLJfQXAOLokcPLAnRUQuOmoWuC6wIsvvjhyuj5PVClwcv9vW1ggcKBqZm5NNnEpAJOAwEHgigKBy+b27dsj+1tW8Huvqm270gP33e9+d0R68wQEDhRh5uYEgQN5gMBB4IpSlWQUETjXMSlBGE4ZkqTKHHF5gcCVC7cPw+06zWl2blcIHMjLwtWrV2neAzQTfsC7lZa8AmdThOiUHzp1iEkbYu4W5TK+s5TvVI108HL6jtT1p0xeLrGevueU71yN19dlehmTbqQItq7yoJ0VVuC4PRjbJnyg5wO+nY8YjSooJHC8H3HqmfguSX23JO9fA87p5ppxFjlOKyJXroG2ChzfhTrYDyjgfCsJotCnfhBSpL4bae3J5afs65saHG9P381aBtw+DAQO1IEWOLkTzVPw+5fPoQSzx8jbghqaz4R/jPnzksiysvPAWYHT6URUhPG8IvnfLLauch/MCisj/OPPbcFtg/00m6oko4jA2R42K3AmRQ3neTsVuEHk0jW3+B8EZdBegevT8Z6rZO1E54Dz9vZ0nrf+tadpXQ21yPVN7J2Eet6xkjQu9/ZO4pxxJzqX3slJRNvbkd6OLou21TrpApgFtw8DgQN1AIGDwDWSaQWuMrjXTueAmx5bV9lLlDdwCrUYVUlGEYFrOu0VuICi/p/TNe9IJzB23ZD6Sr70H1dKgPgRY54q/9BzyA1P9DzuZONyXtZz+jpnHMub7n3b79OH69fMPI9zxw0gcKDxGIHb2aCe+pHsbeyYHWlrVf1o9mhjZ3QH61rgFGpzmeYUapPhuiKRb31UJRkQuNkLXOls98nr96eStiT2eASBA3VwRuBY2h492qLVeNwI3A5t9EwvyOrWI9NLtboVS54ZN+su6OXtshxGCMesn7LzziIgcO2gKwL30UcfjeyDeQICV5yqJMMKnPyM2hQWCFy52LaFwIE6OCNwLG5azHobQ4Hb2eip8VXaWlXi1TPzrJTtnNn5drSY2fWswKWvP7rjziogcO3AClxbg+EhBK4+qpIM9MBB4MbB7cNA4EAdnBE4Hp4KmBlq8VICltzJtJA9svN6Wsp2Hm2lCtyk9eXOO4uAwLUD9MBB4IpSlWRA4CBw4+D2YSBwoA4qu4mhLdfQQeDaAQQOAleUqiSjiMAl88Ax9u5THibvUD0L3/Xskut7574mKwsIXLlw+zAQOFAHpQucvR6Or3eT85oYELh2kF/gojh/Wx5MihCdG07OqgCuKwSuPqqSjCICFw0icl1fD/2A97Uf6HQiWuAiX0laXBYO9HIUBWroUshpa9Qy/ZDTWgQUnoQUXHuaTkIen+4OyTQgcOXC7cNA4EAdLPAPw7xi7+4DzSevwA3zwNlcb3HSXt9Z0/neeK4tGwpcPK3zxCU3VjJcVwhcfVQlGcUEznxmvM+tO6FOeWHzwPk+j3MPnRI5l/fF0OSK8xwKPdMDx+Lm9l2dCiN0++QFfd2jV9ae0FaB4zQigRLigRJazvMWXP82HetxJcbbfS26nAOOxwPX0bndeHqwb3O8nc3/5jlmmWnzv1m4fRgIHKiDTgnc3bt3CwUErj3kFTidr00nS7UCx89Z4B9Hk8eNw5TFIuecChxFxZPzFoHrCoGrj6oko4jANZ3WClz/Ov1g71in/nD9PdpTcuvH454aD9Q4izCPhyceeV6gxYifvMDL2Pxv+9vbdF1J866ax/Js5k3/PeP2YSBwoA46JXDyy5AVELj2kFvgGgzXFQJXH1VJBgRu9gJXGnH+t7AkaeL2YSBwoA4gcB16/12mKwI3LRC44lQlGVbg5PGkTXEeXnrpJXrjjTdkcSnULnAlY9sWAgfq4IzAvfnmXyZmnSXy12iNrxda49NO5qJv+zDw4cO91dCP7APC4/Xih4PXgfwyZAUErj10SeDkfpgV/N4hcMWpWuC6gtzfJgW/d36aCAQuHW6j88DtCoEDecndA6cf5s2XeOsHgsfXFemHevu0Fj/cm6f5miIetwLH8sbTdSAPNhz2qRCynAMC1x4gcBC4okDg8iH3t0kBgZsMt5Hl888/H2m/SfGTn/wEAgcKkVvg8hBFfEF4HQkZ0pFfCA4IXDeAwEHgigKBy4fc3yYFBG4y3EYWCByomlIFbtbIL4SVt3ESB4FrD3kFjv+IGJ6+988mBZHTdWPrmtwHFxcXR/ZTjuT1MxC46WiCwJk0IhENOOeb6+h8byaNTWjSYAziRL+Bq5YJ1HhEPud708vXs79OOmbKYycEbjLJ4xEEDlSNFji5I7UtLMkyeQCSByIOCFx7yC1wfKqfr8skn/x1J74+06E19YMZ6VP55nrNWfQT27rK/Vfuo3I/hcBNRxMEzvWNoF1zQvIcl/wg0Psjl0VqnPdXLXCRETiT780d5oM7R0aL3CT3tZWVlZF9sYkCtx/0aW9vQMOvxH6g2ip/Y+3v7+tccJwHrkySxyMIHKiaTvfAZQUErj3kFzh7Xaavc7qZ6zMjk/+NRU5fo3naS1cntq5yP5S9cPLuNQjcdDRB4JLwI7REAbnR7D9TuT8mJe7Bgwdn5jVG4M6ZB27P5+mwVoG7ePHiiBjfu3dvOB8CB4oCgevQ++8yeQWuydi6yv2Qw0qclDcOCNx0NE3gmorc3zgaL3D8JIZ+oCTNNb2ZLGzxUypY3oJtNa1Ejce1wDksdHu6nB9FFm1vK6lzyHX35KbPBbeRRQpcsl0hcKAMCghckWdMjiL/zuFrlfQ2S7zpQX5ZsgIC1x66LnAcfECXZRwQuOmoWuDk59SmSCLncXAvnJQ3jqYIXFNJtm2awCV74eQ8CBwoSm6Bs48mMrne4lNVkREwPmWlryvia4z4miM+ZRVfZ+SvPanzx9l8cGY9c60ST5d516r8QmQFBK49WIFrazDJYZGAwE1H1QLXFeT+NikgcJPhNrKkCRwHBA6URW6BSz5jUud6i4f8/EidA45FTguaWcY+ezLi5SKbD+50fb1c/CzKMmAZmyZyv38wU+ahB25cQOCmAwKXD7m/TQoI3GS4jSzjBG5cQOBAUfILXAeBwLUHCBwErihNEDj756m+41R8fnJ6Vsj9bVJA4CbDbWSBwIGqgcDN8ftvE3kFLpkHLonu8Y2HRTBPICkHW1d54M4KCNx0NELgOJ+bH1FwzSEnHOhccOvenk4P4vqhzgPHl5voVCIzQu5vkwICNxluIwsEDlSNFji5I7UtLLI8KyBw7SG3wMXC5Ttr5EQDc73lmVP78al7PuUfmWs79XWdavl4C/F1nWaZdZ16xMjfeeG68o+fPI2fJyBwxWmCwNnLRjhHWRS45ClpY5HjTzIKlcy5oVompFllE3nxxRdH9rU8MWuB47tQ+0Go04HohMhhQHvKivt9Xw1P1NAlxyv3DtM8JI9HEDhQNZ3qgZNfiKywP4yg+XTpc5L7YZ6AwBWnSoHr0mch97U8UVXbFhE4b++EQpEPLuhfI1dNV5HjLQ/cNhYWMtt7njcgcKAIC/P8wwiBA0XgXG2gPVQlGU2A03wcHR3J4kYySWSWl5fpwoUL+q5M5vHjx2Lt9iJ/b/IEtxUEDuQFAteh9w+qg+VtMFjoVM9L14HANQOWkklYaWOJk4I3jwGBA3lZ4B0mD8k8cDy0+dxs3jdbzuNV5HjLgxS0rIDAgbxA4NoHBK4ZZAkcAGA6cguczdvGYS4SP5sPzpYbpys3x1tepKBlBQQO5IXlTX1dlMiZUz2g+UDgmgEEDoBqyC1wbUAKWlZA4EBe7LNK0QPXHiBwzQACB0A1QOA69P5BtWBfaRddFjimLX9MQOAAqAYtcFJs2hYWWZ4VEDhQBOwr7QIC1wwgcABUA3rgOvT+QbVgX2kXELhmAIEDoBogcOr987VNyQAgjS59V+YBCFwzgMABUA0QOAgcyEmXvitdxoobBK4ZQOAAqIbcAmfzvulnRxbAd8p7GHgWUtCyAqdQQRGwr7QDCFyzgMABUA0FBI5FzKgYP6BZD3VyX18/LJxLdJ64uEyvw7LHOeHsRipGClpWQOBAEbCvtAMIXLOAwAFQDQUELn7yQkFY6CBwoAtgX2kHELhmAYEDoBpyC1wbkIKWFRA4UATsK+0AAtcsIHAAVIMWOCk2bQuLLM8KCBwoAvaVdgCBaxYQOACqoVM9cPxeigQEDhQB+0o7gMA1CwgcANXQGYH76KOPRnrYsoLX6cr7B9WDfaUdQOCaBQQOgGqAwHXk/YPqwb7SDiBwzQICB0A15Ba4yF+jNT8iZ80hJ87t5vBtqZGvg8t46EemzN6w6utbVzl4GXMnq12/TCBwoGqwr7QDCFyzgMABUA0FBI6lyzd53XzOCBeZFCFqek0PTQ44x4n0+FmBI7WMWm9tLZ5vM8qVBwQOVMni4qL+wWzLj+Y803WBOzpaUP+bWFnhYbOBwAFQDbkFLg9RFJleuRScKZ7iUAQIHKgSCFx76LrAraysDCUOAgfA/FKqwM0SCByoEghce5gXgWN5Ozo6krMbBwQOgGqAwHXk/YNqgcC1Bwhcs4DAAVANELiOvP+uc/Xq1ZHPb57ihRdegDzmpGqBW1tbo4ODg5HPaJ6C24BFcmGBJdKczh0nkxA4AKoBAteR9991IHAQuLxA4KoPboPkdXg8/sMf/nCYJF0GAKB8IHAdef9d51TgtvRf/RyHKZ9p2bG4+i4dHqrh4gL1Xr0zMl/GpcXFuH49evXO4ch8G1vPLurtPtp6Vm9Xjyfm8ynb5OtB4PJTm8C99xwtLy3SAX92O6/R6jsPRz7nrga3gcV+H8uAv+fzHuN6MgGQFBA4TgPiqHhSh84JZ/PBOZzjbW2Y441Ti2jifHARpxiJIr0epyPhVCROnHYk/Z7V4kDgug0f2PgzW1xYPfMZbq3GwtTbMPMXe7S62tMCtXHnji6/wwKmhztq/QVaffeQDpU4mR+eVS1nWggPTwXsVbXuxqV420riuJzXY/Eyr/eqli9+PStsLJR2uWd5O/F6Znunr9nrmddbuKLk8M6GqY9ad+e1S8Nyux3ePi///PPPQ+ByMkuB42n+DB8e7NBr31zS41cS5e+8sxp/xu+MHI/aFEmBK5OHD+dHgtPi2rVrEDiQm/wCp4SL871FcbCi+UraOD2ImW1ywHGuOO1otsw3+eL0PF43FjidOw4CB3IyFLhY1Dh2Ni4pIesR98qtKuFhCdI9ZRt3lACZIQvfu4dbevyOWv7dQyVFSr7ubK2anq9HO2b6TA/YTtzztqUlzIqdFSp+PRY0FjJ+vcOdDT0/KYAseHcObQ9cvL34NYfr2x/+WOC41832Kp6+3iJdUcMXXrgKgcvJ7ATuU1rqvUKfHhwqGf8mLS2t0tZzy7R06RW93BOvfKKkbpk+ORjfM9uWgMBVExA4UIT8AtdwIHDdxgoci5mVnHSB4563Qz3N0sa9aL1eTwsa956xwOnPX8mXFSeeNvOszFlxyyNw3PN2KmR2ObOPnW5vkXvblMCxjI0XuNP3BoGbntoE7tF79NzykhY4FrZ3Hprr4t5T0nblyqL67K4MP08WOO6Js/OXEvPaGJUK3HvP0+W379PDQ263BT0uX7+KeP7CCt2PP8NZBQQOFAEC15H333WswD3aik9B8SnPR4lTqEq0eL4UOCN5VozMKVQtZWo7RpzisngZczq1NxQ3u/zIKVR+vUyBE9sbCpwRRnkKdbjtM6dQTRlOoeanPoF7pHvh7P54oP4AWIpPx/O4PYXKp0utwHEPHJdxb5w8HrUp6hK45y8s06WXP6b3nr+gZW7hiZeH4ruybL4bH7/6TTXeo4Od1+nKW2q9LbX+5R4tqTa/fHlJDRforfsP1Xqf0bL6fC7zMmrbF1Yu6eUWFp6gl29/ouctXH6L7r8Tf6ZqXL7vqgMCB4qgBU7uRG2Labl+/ToEriUMBW7OggWRr5/DTQz5qVXg5jTqFrjlpcv0tpKw15/kXrJD+uz1J/U0yxyPS4F7+eMHdMDb4ek7pvyd5y7Esn1ZCx0L3Mu3H+iet0sv3z7tgXv/eQgcaAWd6YFj7t69WyggcO1hXgXOBgQuPxC46qMugVteekLLGAsaD1noHhwYgbPLDAXOCtsEgWNxs+9hrMCpee8/f4FWlDQepLz3KgMCB4rQKYGTX4asgMC1B9tTLEkrayq2rvIPiazg984BgctH1QLH2z08PJTFrUXub5OC3/uNGzfojTfekJspBdzEAIED+YHAdej9d5kuCZzcD7MCAlcMCFwx5P42KSBw1QYEDhQht8BFEacIsTndTJmvRnRakDNEunwWyC9DVkDg2gMEDgKXFwhcMeT+NimqFrgunJo+D9yuJycnshiAVPILHCff1XncTO42TtbLosa53JI535oicPbOMBnJZSBw7SEpcO+/v6EOdAt6XB4weT/Vf2pEvL/6en91OO+gH8b7MO+3Jgdhchk9rdbj5cJ4Xzf7c3nYusoDflZA4IpRt8D9+Mf/mn7728XEEqdE/JkFgd6XBmp/C9W0q/YxLo7UtOs6enyWyP1tUtQhcG2G28jy+eefj7TfpOBnxkLgQBEKCJwTS5sSuOg0ca8e8vRQ7Ez5LJBfiEnyxgGBaw9JgfvZz35Gf/u3F/W4LbMYYTNyxommbbJo+0cGo/8QMQsnljE9yfy/6VUu/w8RW1e5H9rglCGyjAMCV4y6BY75zne+c2baYgXOCwfku67aN0NTpnDVfjYYRI0UOH5Q/YMHD0bKIXCT4TayQOBA1eQWuDYgvxAc4+SNAwLXHpIC94c/EO3t/a963JYVocwngBTB1lXuhxw23xuLgZwHgSvGLASuzcj9jWPccbMugbOvN479oK+kmHsyp/tO7O/v04D2aXu73CNBss4QOFA1ELgOvf8ukxS4JGllTcXWVe6HHFbg0iQOAlcMCFwx5L7IvW92X5S9cHUJHO/qP/nJ/6HGHp9dIGY/CGigJK4fnlC/75K3x0M+RR1SoKSs73rkeT6dhIEuj7b7FLgOnQz26dqHu7Trc7lLjrcnN30u7HecgcCBqum8wE0KCFx7sALX1mCSw2TIU/3yDw4IXDHqEjj5ObYpkiTLk/KWJnF1CZyt48nJUXL2EO6Bc1yf9k4G5HseuW6ohnsqHLrm7lF44imRO6FravpDf4+Cfl+Xnajl+0r29oJtJXiOWq9+geM2vXfv3kg5BA4UBQLXofffZbrcAyd/MDmSggCBK0ZdAtcVsvbF5B8UdQlcW0kej9IE7uLFiyNtagMCB4oCgevQ++8yXRa4rIDAFQMCVwy5v00KCNxkuI0saQKXlGLZCweBA0XJLXAmPYMe0bfE891+a3yXXnS23F/ju1I5HUO5F4fmwf7Q5Q0IXHvgzyl5cLSklTUVW1d5UM8Ku79C4PJRlcAtLy/rYRGBM8fG+o+FRZD726SoQ+Dka7YtLFLgkr1vab2bEDhQlIW8AmPzaY3L/2bL19T0GufakhuoGJaxaSLv+wezJb/AmX1zWuRPrU1gXUZKEVtXecDPCghcMaoQOJa342Pzg/ujH/0ov8DFaUTS8sDpY2iopt243PXUvNPl6kLub5OiDoFrM9xGFilwWQGBA0XJLXBtQH4h8kSX3n+XyStwnJiX4T8o9DDO8aaHcR7DYW44/o97jqMBRTZHnFhPJ/+N/zg5L7auVsiKBgQuH0mBk204bbC8qcOljuXlBf3IpzxwrjfOO+g7vB+pfWoQSx3xkBP5mn1S74MDjyLX1Rfe83J1Id9rVkDgxpM8HkHgQNUsyC/nPAZoPvw5SVljRspsTzH/aMZDnaQ3cmidE0/Hvcfcr8ZD7jHmvNNDgRPr2SeQlJGcmuvKP36yFzhP8PuHwOWjST1wTefFF18c2dfyBAQuHQgcqBP+k3Lm8EGxIVUBDSW3wDWY89QVApefKgQuSZFr4LrISy+91ACB29ffB04nsv7hLn3o9nW+t1l/QyBwoE4aYU0QOJBFlwTu7t27hcL2FEPg8lGFwHEPHB+njo+POydwcn+bFM25Bs4IHOd385Xw+GrIed1OZvwdSR6PWMjk2Z6sgMCBIjTEmk7vyAEgjS4JnPzLOysgcMUo89o3G3z6lK99K3oNXBuQ+9ukaI7ANRNuI+b27dtTJXvm9oXAgbw0xJjMgRECB8bBP6L24Jgkrayp2LrKg3ZWWImAwBWjzB44exND166BY+T+NikgcJPhNjoP6IEDRWiMMfEPFADjgMBB4IpSpsAl6dopVLm/TYo6BE6+ZtvCgmvgQNVA4EAryCtwNuG0HyeW5rQiPG2ST5s7SmeFras8cGeFFTh+4D1jRY4P9FWcLuxalCG+9ho4pojARYNIpwoZ5npT+6HO9TYw+d4C3jsjns9pblxd5rouhbwfq+X5Ye0nUUDhCecfCchzr5Gnykp4S0Pk/jYp6hC4NsNtZIHAgaqBwIFWwPtH8uBokWVW4NY5HUjE+eBMommT8o1zvJ0/Hci02LrKA3dWJEWEJYJFjoc40E+GZYOjDIGb9hSqy3kHowEF7jo5Srw4cS8H18j311V4KgJdR1+Jmx/wOP+x4ap1PXJjgeOh5/bVMn1y+6F+KHtZyP1tUkDgJsNtZIHAgaqBwIFWkFfgmoytq+whyhtliMg8YE+dNkHg2oAUiUnRFIHj9CFeyE+0SP9s97e3KUqIkJyuiuTxCAIHqgYCB1pBVwQOiXyrpwqBm/YUahuQIjEpmiNwAQ2UxPEpZs7/tncyoH4Q6nHPc2nPDyjwXfL39si/9m3aVdMscP2+S6FaL3A55Yjc6vlJHo8gcKBqIHCgFXRF4D766KORA3eegMDlBwJXDLmvTYrmCFyfHNeIm+/vUTiIlJSxvO2R6/Z1fjh3fV2Nh7Tn9MlT0yHni/M88hwe90o9DW3hNrJA4EDVQOBAK7AC19ZgeAiBq54qBC7tFKr8jNoUSeS8SdEUgcvDtpK283/yxUi2LQQOVA0EDrQC9MBB4PJiBc6Ol9FuVuA4iS9LHBL5vkErKyu6LY6OTNtMy+PHj/WQt1WmwM0CbiMLBA5UzfTfupKBwIFJQOAgcHmpQuCS4BSqEbjTp+eYODo6kpvO5MKFC8PtWIGTr9m2sEDgQNVA4EAryC9wEUWJTCFpSUN8zi9C6fOqhOsKgaueJgmcy+lsQvP6AacJ8X0aRAEFUbl1Og9yX5sUaadQT0Xu/KAHDgIH8lPOt64EIHBgEvkFzied7o3H4sS9EZfxDynn5PJ5PieIc2iNk/zWmNiX6wqBq54qBI5vYuDTqEXTiLjxPui6ai90jMBx0l7HH+jEvJwfjvO/RQO1L6ohP5Dd9UO5mUqR+9qkSBO4MoHAQeBAfiBwoBXkFTgWMkcn8OUfTn4SQyxwWt74R9TRT2fQT2jgJL8QuM5RhcDZa+DsdXB5r4HjpymwmK07IXmxwEXu07TuH+snLuj9MilwSt7q7pyT+9qkgMBNhtvIAoEDVQOBA60gr8A1Ga4rBK56kgJXFtP2wLUBua9NiqYI3Nk8cEp890w+uHAQUrAdUd/1yPN8OgkDXR5t29xv+3Ttw106PomUKEW0zcvy+uoPuZNom/aUPJ3nW8ZtZIHAgapplMC1JUD9cLsnD46WtLKmcp668vuHwOWjCoFjWOKYItfAtQEpEpOiUQK3rwQu4OfCDijQjyAb6Pxu/X6oh8H2CV1TYvah5+jHkNncb4ESpBPa1+P8hIbrrku7nCPO5ceX7Z0rP1zyOw6BA1XTGIFrC2VdrAuK0SWBkwfurLB/OEDg8lGVwFkgcLMXuNLYPk3yWwbcRhYIHKga2EhByrzjCuQHAgeBywsErhjyDENWdErgSiZ5PILAgaqBiYBWAIGDwOUFApef27dvj+xvWcHvvao27koeuO9+97sjzzPOExA4UAQIXEHQAzcb8gpcFEVn8rs5cc43Pc/mF5kRtq7Jg/3i4uKZRKY2ko9qgsAVoyq5sBQROL4LlXEdvhOac8Dxhfa2zBmOz4omClyb4fZhuF2nedwatysEDuSldSZy9erVuY95JLfAcZoGNdRpQtS/Nc77tvYkDTiVyLqjxp86111m58HWVR60pbxxJOdD4IpRlVxYiggc54FjSeM0IX6g9sLQJyc0dzpyChsI3FkgcBA4kJ9WCpzc6ecpIHBnkWU6t1ucrJdFjvvcdG44x/S+1Zn3TWLrKj9T2QsnD/wQuGJUJReWIgLXdCBw5cLtw0DgQB1A4FoWELizpJU1FVtX+ZlyWIlLO+hD4IpRlVxYIHCzF7j9oK/+GPNUuMMe9YDTiUQDOkcWkHPD7cNA4EAdtFPgdjaox70VvQ3aUTv91uoC9Xo92tgZ/UKcN1bV6/Q2dmhno6d+YHsj80cirpuZ3tLr8jivW0b9IHBnSStrKrau8jO1IU+d2oDAFaMqubBA4BoicK6re9TN48f2KArVtHdCJ+E2uXvHOj0I54IL3b7OBcfLVA23DwOBA3XQboGLpUhLVixwGz1zGmp1y3whhtcUba2a8dWteN2FWKh2hsuwbMn1zwrc6pnlZT3suqc/xEmBW6WtR2frZ7Zp6mS3a1+Xt8vjvO2eXofX35l7gWtrMMlhkYDAFaMqubBYgZOfU5vC0l6BCyhQgsY3gnhKzMx1hgO65u3SD5SwueHJWYHjZWromrNtC4EDddBqgbNSZQWLJcoKlBWmoVAlBG64jBrn3jvuxdPb3DiVM7v+6oIROC7jIS9vl9my27Tb7W2k98BtmTpyWXL79jW4/HS7piwpcEM5VK8x7wInSStrKrau8qCdFRC4YlQlFxb0wM1e4JoKtw8DgQN10GqB414rFhsWMCtww16wRK8Why0/lbRevEwsdL3emR44M+9U4Hg5Ht9JbNP2oOkeslUzX29H142/kKc9cGbexpntD6UtWdf4dU8FLl4mPl0MgTtLWllTsXWVB+2sgMAVoyq5sEDgqhU4+XptCwYCB+qgnQKXsuOfO7ZWS7lGreyQ185B4M4iy5J54GTeNzldN7au8jPOCghcMaqSC0sRgeP8b/b03iQifpZnFKSmuOFThVXRRIFrM9w+DAQO1AEErmUBgTuLLGNJ06lCfIfW1ZCTpzL8v6+meb6vU40k0/3WA9eVH0Mks6/nCQhcfqqSC0shgVP7mh+ZPHCuG+nPUO+Hapo/zoAvxPdDCn2ezw9dd9U8jzzHDPn6LX4gOz+s3TxsnR/UbvLIlQEErlzs8QgCB+qgdQLXhYvZzwO//3mkS+9b7hN5AgKXn6rkwlJE4GzPmxU4/oPC13IWC1zA46YHLnK/TXtK0nzPOStw6+tq3ZA8z6VQLQuBay7cPucBj9ICRWilwKVx3i/OLJAHxqywp9IAAOOpSi4sRQSu6bRV4PguVJbevhLeft+nvRPu5QzpZM/X08H1b9O1D3fJdX5A3l59QsTtY8HD7EHVQOBmiPwCZwUEDoBsqpILCwSuCQI3mgeu7zv6OsLt/jXa4xQiSoTCE08JXShXrwxuHwsEDlRNqwXO7vh2PIl5JmbEI+YxSskhP2YpftwST+uroXhoxxV8nRRfq8LP0Vzj5fX6TuIVzo/8AmcFBA6AbKqSC4sVOPn9bFOch5deekmLRhXkFbimkmxbCByomlYL3Jtv/iV98YV5C/KgFDlrtLa2ZiRMyRgLmq/K9DUoLGRq3InMhe0sdObalNOL21nWeIyfo8kXwPPzNWmod+Ugv8AcNm2ILOeAwAGQTV0C1xXu3r2bO/i98404ELh0kr9DEDhQNa0WOOaVV/5LPUx+ccpA3ykmC0tGfoGHOd/GSBwEDoBsIHDFkMeZSQGBmwy3kUUK3MWLF0eO8ffu3RvOh8CBorRe4CzJL05bSH655Rc7TeIgcABk0ySB03ng+NbRmKx8cLMgeYzJCgjcZLiNLFLgOJISl5Q3DggcKAoEbobIL3dWQOAAyKZJAhcNIvKDSOd485XIRaFJ7Nsk5HFmUkDgJsNtZMkSODkPAgeKAoGbIfILnBXjBM6WIxAIE1VSWOBcvu6Wb4KKyPE5ia9carYkjzErKysjZwGSsgGBmwy3kSVN4DhY4mTvGwcEDhQFAjdD5Bc4K+yPkzy4AgDqo4jAtYHkMSZN4B48eDCc3xSB02lEHE+FO0xq3O+7FHJ+uBkKELeRZZzAjQsIHChK6379IXDp7x8AUA9dFjgbafLG0SiBE3ng+JmxvgrO/eY5ff1Ei5OTers7uY0sEDhQNZ0VOJ3nLR7mvZuUH4TOqUJ0frgakF/grIDAATB7rMDJ72ebIomcxyFPndpojsAFWthcFjglb3xdYeC75LmqbH2dHB5X5fXqGwQO1Eu3BS5+mPmazfXG05yMN5ETTgtbnCuO14HAAQAmMQ89cOOiKQLXVLiNLCxk8trMrIDAgSJ0VuDywD1u/ISFWSEPjlkBgQNg9kDgqhO4LsmLbLs8wdLXpTYA1TLXAjdr5Jc3KyBwAMyeIgJn+/L5wesyB5ycnhXyODMpIHD5kW2XJyBwoAgQuBkiv7xZAYEDYPYUETi+Rovzv11TwzXHp4GSNl8FqxtfgB/6Pnk+X+JR3wPXJfI4MymqFjgAQH5aKXDyoNKmSCLnydv35UXEEDgAZk8RgbPPWD5R0hYFfGF9SE5oBC4KHVp3Q7VMONPccPI6rKyAwAHQDFopcGlIOWoDUuCSEifLIXAANAMWOO5J6wryOJMnuA0AALMFAjdD5EGRAwIHQLNheZG9UvMYAIDZ0lmBs/nf/MjXKUQcx0zzON95ytefzBopaFmBAycA7Wd5eZmOjxfUcEENj4flXGan7TLjphl+asLR0YKKo+H0ysrpNMPzuQwA0D1a980eJzBS4EhfGGyeQeg7PIy0wGmJi/O+zRopaFkBgQOg/ZxH4HgdC8sZH8IhcADMJ637Zo8TmBGBawFS0LICAgdA+2ERM4feUyFjQbPTLG1W1pLTyeVZ1uw2rLSZ3rhTYUsuk5Q6AEA3gMDNiOvXr08V494/AKAdsKzZa12TPXDJabvMuGnGPng+2QOXnGbs6wAAukfrvtldEhjZw5YnuvT+AZhn5J2s8s7OrOm0srRtyjIAQDdoncABAEAXkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA3gMABAMAMkGIlZSxrOq0sbZuyDADQDSBwAAAwA6RYSRnLmk4rS9umLAMAdAMIHAAAzAApVlLGsqbTytK2KcsAAN0AAgcAADNAipWUsazptLK0bcoyAEA30AJ39erVuQ8AAKgTKVZSxrKm08rStinLAADdYChwjx49mtuAwAEA6kaKlZSxrOm0srRtyjIAQDeAwD2CwAEA6keKlZSxrOm0srRtyjIAQDc4I3C9hQU13KJVPRwVnczY2aDVrZTyhgcEDgBQN1KspIxlTaeVpW1TlgEAusGIwC1w9DaGcrPRU9OrW/Roa1XP4zIeWlHb2ejRlhpurS7Qjha4Hb3OToooNTUgcACAupFiJWUsazqtLG2bsgwA0A1GBG4oNrGw6RgRuFUtbTzO4maX27I9cHZdXi9FmJoWEDgAQN1IsZIyljWdVpa2TVkGAOgGYwWOe9ZYwnopPXBDadOCtjMUOHMK1fTA6XU3dkZkqYkBgQMA1I0UKyljWdNpZWnblGUAgG6AmxgeQeAAAPUjxUrKWNZ0WlnaNmUZAKAbaIF788039YQUmzbFebDvHwAA6kKKlZSxrOm0srRtyjIAQDcYClyaBKWVNRVb17t37xYKfu8QOABA3UixkjKWNZ1WlrZNWQYA6AadEzjZM5cVEDgAwCyQYiVlLGs6rSxtm7IMANANCgpcRI7jiDKDr8oj35fFuoxL/UjOideRhVMCgQMAtAkpVlLGsqbTytK2KcsAAN2goMAp6VImFvkOOT7LnK8FjIcsdjxkKWNZ0+ORWXaN5/uxzMXr+TzurBkhjHh9Xy87LRA4AECbkGIlZSxrOq0sbZuyDADQDaYSONNzpoYscpEpYxFb0+UsY/E8hwXOSN4aC5xeRq9J3Fm3trZmRE4ta7c7LVkCZ1OgyIDAAQBmgRQrKWNZ02lladuUZQCAblBY4PISca9a2nnTisgjcGkSB4EDAMwCKVZSxrKm08rStinLAADdoDKBq5txAjd8mgQEDgDQIKRYSRnLmk4rk9sEALSPn/70p7m8ZO4ETkocBA4AMAukbEkZy5pOK5PbBAC0DwgcBA4A0GCkbEkZy5oeVwZAE/ibv/kbWQRyAoHLGRA4AMAsKEvg7DFsnuPk5EQ2DZgxELjpgcDlDHsAYJK9dAAAUCVlCBwg+slPfgKBayAQuOmpTOD8NU7Y65iUH4n8bY7v01oix5smMilE9PJRpNZ9UqcV0TnjSr5DFQIHAGgzUtDkdNNhkZoFELhmAoGbnsoEjnO9DZ+gEJlkvHaatc3meNNPX4hFTQ85/xsvp+WO88RVI3Cyaz1vAADALJHCJqebDgQOJIHATU9lAseU+QissuC6fvTRRyM9bHkiT0MBAAAYDwQOJIHATU+lAtdEIHAAADA7IHAgCQRueqYSuLYHc/fu3UKBU6gAAHB+fvjDH9LR0ZEsrhwIXDOBwE3PVAInSStrKrauUuqyAgIHAADnBwIHkkDgpgcClzMgcAAAcH4gcCAJBG56IHA5AwIHAOgCV69enWmwwD333HMj5VXHG2+8QS+88AIdHx/LJgEzBAI3PZUJHOd5k9g7Uk2KkHT0nGj8/PMCgQMAzDNSbOoOFilZVkdA4JoJBG56KhO4tTWT983n5LyxsHFiXp0HLi7n3G+cD87mftOZ3yLOBeeY5L+RXZbM/Hid8wCBAwDMMwcHByPHtzqD89bJsjqCT6Hye4fANQsI3PRUJnBazbS8mQS9PHRY6nxO6mum9dMXEgLHwqbLnCfNUxhGBM6scx5sXeWXOysgcACALgCBg8A1CQjc9FQmcE3F1lV+ubMCAgcA6AIsMc+tLNOlVz6hnde/RctLPTo4HD3mTRO83aXLb4+UJwMCB5JA4KYHApczIHAAgC4wKnBX6PDRZ/T6t5b1852vvP1gOM7x8OCQPlPLvfLJQzrYeX04f2lxgS6r8YOt58yyl1/RZTz+4N1n47JRmYPAgSQQuOmBwOUMCBwAoAskBW5leZGeUMP3nluh5Vi+Fhau0OKlV+jws9fpW8tLqQK3vGSXvUxvv706lLVhD9x7VuogcGAyELjpgcDlDAgcAKALJAXOiNsT9K4eck+cOd7p06pKwlakwKky7nVbXlLi9uD0Wjq9HVX2bOIUqi2z27QBgQNJIHDTA4HLGRA4AEAXyH0TQ6IHbmTeOQICB5JA4KanMoHTd4tyKhDHVwNH301qps1Q322qyuvG1lV+ubMCAgcA6AKHh+UKWdEoQ+CmgQXuxo0beBpDw4DATU9lAqfTf9g8cDr/mx/ngzNpRTifm5a6mrF1lQeErIDAAQC6AAucJO0YXhUsUufB1vXzzz+nu3fv5g4WRwhc84DATU9lAtdUbF2loGUFBA4A0AWswD18+JAODk6Ph3VRpsDJ4/SkQA9cM4HATQ8ELmdA4AAAXcAK3B//+DVdufJdLTR1HsMhcCAJBG56IHA5AwIHAOgCpwJH9G/+zX+hhOZ45Bi+HwQUBBENBgPquz6FJ2bohSFFg5AGYaCnB/tqGPi07u3Snh+Q67rkeOGZbUkgcCAJBG56phK4tgcjy7ICAgcA6AJW4L78ckC//e2/VGMpAte/Rj/YOyHlbeR7Drm+kjYlbix14YlHnuPqck8Jmx+eUKCkKFTSdxIvMwkIHEgCgZueqQROklbWVGxd5Zc7KyBwAIAuYAXu8eOv6R/+4b9XYjYqcOnsZ8pZHqoUuAsXLgyfICHnQeCaCQRueiBwOQMCBwDoAlbgOCfaxx//ZyVw9R7DqxQ4K28c9+/fPzMPAtdMIHDTU5nAcf63caT9Def7kckXF6XNLQ9bV/nFzwoIHACgC3QpjUjyGJ3sfUuTOAhcM4HATU9lAre2tnaaB07nf2NJM/nfOJnvWjzPlHOOOBY4tVzEy3AeuTU1qpZJrFcGtq5S0LICAgcA6AIQuGoEbn19na5evTrXcXR0JJslEwjc9FQmcBEn7NXyZhL26iFLGs+LrNgZKdOJfR3TA6ef3BCvywKniofrlYGtqxS0rIDAAQC6QFcFLivqEDj5mvMU165dg8DVTGUC11RsXeXOlxUQOABAF4DAQeCqCAhc/UDgcgYEDgDQBfII3H7QpzAc6Bsc8sK54zgvXBClrxQqceI53Ra49+n5iyvmFO7lt+jew4ORekwb71+9SCurb+nxz/76KbqwvEj3HpjtJ+dxJMfT4uKFZXrm5/dGyovGhdWf0xdxHSBw9QOByxkQOABAF8glcFPmgXNdhxzvRC9zolYO+n21zoBCt097aplALcPHUV6PhW4abF1ZyOxxOW/UJXAPDg6VYF2ht+49VGWf0etPmuvzLr91bzjO8fHGk2q5Hj3c+Wt6SknVg4PP6K+fMvMfHKjfnvevxjL4Mi0vmnVY2i6uXKKX33qFrqjt8Tqn8+4M17/3yWtqmyyTT9D3b9830reySAvP/FwL3BNPPKHmPUM/v/eAvnHhEj3zTE9PP/PMsh4e6N++z2hlyWz7wdZVvdzy8hJ9/6OPaePJi6ZuanvfUNt79tlnIXA1A4HLGfYAAAAAbSaXwPGTGPoBDZSE+UrewsHAiJyrJI7FzXN1ecTLuT65oRG4qP9tuuYd62WOhcB5nqfX5+Mol59H4F566SW6fv16oXjjjTdqEzgWm9tKtA5VXT97nSXtCm09f5FWLr1MKyxs7z2vJGxpROA+eY171tSyVy/S7fuf0utKxm7ff6i3c1Vtd+my6Vnj7fD85StvaaFLzuM6nI4/0kK3csVIm+110wL3/Y/oKbVeTw1ZwHg6OWSBZOn7+RcPVH2+QR+9/axe7/6nr+t17j98j5af4R64O7TS+z49+/wLELiagcDlDAgcAKAL5BG4KinrFGpR6jyFyj1wK0tP0Mu3H9D7LG5Ll+MerUdG2KTAxdPvPHd2WQ59elSVPWcl7f2ruiePBYulr/fy7VSB0+KmxOr5ixe0aHHvW1LgePyqmndJC9sl+v5H97W4PfPzL7TY8fbfV+LGAqfr877pgXt4+L5e51TgcAp1VvzZn/2Z/mMhi8ICN20euLR5ZcJ1lX+Z5Q0IHACg7bDAyT9Q64wf//jHI2VFg2nmNXCjr5san/01Pcm9XA9n+1mUGRC4+qlM4HTij8g3IhcPTYoQ0jndOPfbUPJ4fpwHzolzwpWU9m0ErutHH300svPlCQgcAKDtdKUHDgLXrIDA1U+lAjdVHjguU/OrgusKgQMAzCsQuGoEbtz7qrNty4Lb9u7du7njxRdfpB/96Ed0fHwsN5UJBG56KhO4cxE/oaEKuK4QOADAvAKBg8BlUbRtmyxwfPmTfGJEV2JlxdwwI8tl1CtwFcJ1PY/AWduV1mtvcmhCAADAOPIJXEBunEYkcB2dKqTv+hSqAtf3aN3bpT3fJc/zdYoRO88sG+r5fCdrFPrkeKHazmluuHGikxdb16KSUafAPXz4kL788is9Ptq2zado2zZd4JKPVJu34Pd/RuDaHucROEbnvomjaUDgAACTyCVwQZ/WHJPLjdN9cP62wF036UJ4WoV/JkWIyRXH+d/cfqjns8C5vq9zxw3mTOCOjo7p6af/B3r8+Gi0bRXRiUlqPI4whwjt7++rbezT9nb5F4wXbVsIXHNjROAkaWVNhet6XoFrMm2oIwBgduQSuAT2CQplMQ8C99VXRB9++N+o13o82rbbm1qOdx2Xws1NCpT0bPevkbt3NKxbeMzJkvt6GLqbdN1hQd4jT8lyfzPU64Tb23R8EtYqcN/4xjdGyjjaIHAv/MlFWn7mZyN1HxdFl+f0Lbz8b++X9/SNMgICF0cb5KgNdQQAzI6iAlc28yBwX3/9R/r1r/8lDQbpPXBNJ61tWd74rNMXX3wxMq8NAnfx0vfpV/rJGJyg+Gn6xkpP57RjUbvHdwO//wL987/6Fd3jRMVqOBQ4Vf4nF5fp3gN7x/BndOGKErW3n6dnfvZbus9PzNDlVuDu0IXeX+nky7KdZhFTCRwnCJGpQDhNSBT5epjGSLlOP+KcLTsnXFcIHABgXoHAVS9wf/zjH+k//af/pVMCl7x0SM5rhcAtP0M/+605lfr+C3+ipw+UjP31098wvWZK1P7qV/foAScqThE4LWpq3Dw+7Gf0q1ef1Ns7Tbp82gPHy/MyTeiNm0rgOFUIp3njfG8cEacSSYRZSAlaLHla+OJyvbwuiVOQ6DQkZsjlvK1p4bpC4AAA8woErnqBS1Jn25aFbFvb+2ZD9sK1QeCsiHH9WaxY4qyM6fchBM5I3oLuabMC99n/+bRa55/TEvfAqW3wfLs93oaefuav6MLSgu7N+2LYaze7mE7gVKyxwNkcb0LgWNLWeF4scDy05Xp5FjUtf6c54yBw2bShjgCA2QGBq07g5Gu2LSzJtpXyliZxrRC4lPc7DzGVwDUVrisEDgAwr0DgqhO4NOps27Io2rYQuOYGBC6ONshRG+oIAJgdeQRuP+hTGA4okf0jLg906opkWpAkkSqfNJ8ZJzp5sXUtKhkQuPwUbdsmCxxvn/PyyTq3Jfi79Lvf/U6+rdzw53JG4NoeEDgAwLySS+D61+gHcSJfP87xxsl8fbevytS46+pyTw0591vouDr3G+d94/kQuFNG2nZ7k/ZU205oohGqzPmWRtG2bYPAMb///e/pV7/6TI/Lz6XJWIEr+rnwex8ROElaWVPhukLgAADzSi6B48S9/YAGyuB8TtA7iChwfXIdfvqCEjfP1eVBvz8UOE7y67pmPicAHsc40cmLrWvRH7PmCNw2BUrETsKA9o6VyO1vk7en2pjbnMtV2232fV2+Gfh07cPf0K4fUL/vkuPtndlWVRRt27YI3NdfD+jf/tun9Lj8XJoMBC6G6wqBAwDMK3kErkrGiU5ebF2L/pg1SuA2A/I97r0cqPFNJb57tOf06ZoaHh2zNO9RtM1Pv1DSvHdMe8G2edqFW4/A8XVTRaMNAvfll0S3bv0zPS4/F2b7pvosHE8Wj4V7RvmygVu3IvXex//Rcl5qFzjHMXeRcoevuXs0SURRxHeamrtVh6V8t2lUbRexrevdu3cLBb/3NshRG+oIAKgPe9egffA1C5w80NcZP/7xj0fKigZT9MesKQLXRb773e+2QuC++uor+sUv/i89Pvq57NPRkdk3eB/hywTCI35ixiYFR0dq/Ej3irq7j/X73L95k8Jbt2hTeU7nBI71zCbu5WS8ZmiETk9bueNltbjFiXx9k1qEl/XXnqQ1NcOuVwa2rvKNZgUErtvYzxeB6FocqR8exg4fPHiQ3PU1acfwqhgnOnmxdS36Y5YmcCy0jx8/Hk6fh3Hvq862LQtuW9mJMSnacgqVBe7v//5ZPZ72udz0PfL4ek+PH2W2ORS4zV0WuF3a5B668EjN31WCt0m7gRI4JXquu6u+X7MTON6P//Ef/3Gk/BwCZ3PAmTA53kyvG+d106JmJW5tzeR+4zIta5FehvO96fxw8XplYOsq32hW2INh02lDHZsI2g3MC/N+CnV5eVmtzT9pC3ThgumdlNKbJ7hHk7l48eJwKF+zbWEp2rZtErhf/vJ7ejz5fpvOJIH70z/902Evu5S4qQWuCMMnM9SAratshKywX9qm04Y6NhG0G5gX5l3guAfO/uBZiZsWmdw2jTrbtiyKtm1bBI5F6Nat/0ePt+lzGSdwSXmbmcDVia2r3AHlF5EjOR8C123QbmBeyCNw4/LAjWefAn2XZEgnGSs1QeCqYNz7km3bBuQNCnmiDQKXpE2fyziBk84iJS5V4NoejCyTjQCBmy/QbmBeyCdwAQVBpPO59V1f3y3JQy8MKeI8b2Ggpwf7ahj4tO7t0vGAl4/0epzygvPGscvZ9S3jRCcvtq7yxywrIHD5KXqjy+3bt/XNKRC4ahgncFmRKnCStLKmYusq3+gkeeOAwHUbtBuYF3IL3Jk8cAOT0NcNKUjkgYt4OU51EXLS3/gJDPt98tbX9bKc08yubxknOnmxdS36Y9YUgeM0IpGKvusq0fV1LrjrrkdHey4dc9X2t3VZX5V5nk/HSpZ1XrgzW6kWCFyzgMDF2LrKN2oDAjefoN3AvJBH4KpknOjkxda16I9ZkwRO535zlAj7e7p3MlJ1ijb/Wzo62afNzb4S4mMKjz0KtpXcKWH+UC0HgZuOIgJXOA/crVsUqPd8KzqmtCwit27eLOVzq13gps0DV9bdpuOwdZVv1EaavHFA4LoN2g3MCxC42QpcG5hXgdO53tTw5qZLYXCTAtch/8a/oOsf/Jo2/V06CgPaPTrR8531b9Nv/E264Xp0MziiI/XWo+OQglsRbaoyfvyc594gb/eIjqNbar1jim7d1Imai1K/wHFuN04DoiTOd56kNcekBnE44ZvNEUcmD5xJMcLCZ+bzPC6vAltX+UazAgLXbdBuYF6AwEHgsphXgZsmD1yk3vMN74/0WL11vrzgurur1wvdm3QjvpTA89S2lLgFU/bIscB973vfG7lxJE9MJXBT54HTGX0dsbXysHWVO2BWQOC6DdoNzAsQuOoETr5m28IyrwLXVFjgPv7448KfCwfvl4UFrqnYuso3mRUQuG6DdgPzQlcEjrdjj8t5o2qBS6POti2LoqIAgauW5ClU+RSMSTH1KdSmYusqd8CssAeAptOGOjYRtBuYF/IIXDKnGz+sm0/7hEp8pjn9IxknOnnhur700ksjp4qy4o033oDAVUQbnoUqf9PbEnxnd6nXwLU95Bc7b7ThR74NdWwiaDcwL+QROE7Mq1OCUHyNspIefdek52Qm6s1inOjkhevKPT5Fe4n4dX/1q19B4HJQVBTa8iSGJG36XEoTuHmPptOGOjYRtBuYF/IJXHWME528cF3bLHBpeeCik5D6mybfG09v3/hz+tC9rqT5B5TIgVwbRUUBAlctpQgcaD4QkelAu4F5AQI3a4HbJEfJG9/cxz2arr9HfV+Nh8e03b9G7t4R7W9ukr/Jdy56SorqN7iiotAVgdNyfY79Y39/X19ycOtWuQnRWi9wNsEumAxEZDrQbmBegMBN/wM9iXHvq862LYukKHzjG98YeVIRxxdffDFcpisC96Lr0Jrj6XxwJ6FP25vXydu8qacDFf7Nmzq58g0l3x/wE0lck2rk6GhA+2peeOsWbfo+BA5MB0RkOtBuYF4oKj5lB9+tKMuKBgSuWuZT4PaViJ3Q/vZN+uCDD8hxXN1DagWOc8HxuBY4Vf6Bv6vzuiUFjvPCbbouue6u2Pb5aL3AoQcuHxCR6UC7gXkBPXDVCZx8zbaFJU0UkvIm53VD4JpL6wWOgcBlAxGZDrQbmBcgcNUJXBp1tm1ZpIkCBG52dELgQDYQkelAu4F5IY/A7Qd98sJwmEokjf0gOJMXTk6PY5zo5IXrCoGrlnGiwKdTZRlHGwRO1rktUVoeONB8ICLTgXZrHlevXkWoKJt8AqdkTElcPzzhCeq7Pg14GIR6nB/UHfoB+fywbyV6694u7alp13WV+J3oh4CPc79xopMXrisErlqKikIbBE7Sps8FAjcnQESmA+3WPIr+QHct1tbWZihwfXKUqIUnAwrUuK+ELVJSF6gyzwv1UxmCfl+Ne/ph3a6SNs89nQ5PPCVK6QY3TnTywnVts8Cl5YELlfhwa3E+uGBTteOHni5Pb8HqKSoKELhqgcDNCRCR6UC7NY/Dwzv02jeX6N2DQzpUB6Nnl5ZosfcqPXrvWeq9+ikdHJ4eqJaWFnXZe88u0dLqOyMHsjbGLAWuSsaJTl64rm0XuGBz0zzZwt8zkuy75PrHWuSCGzfIUdNcDoE7P0UEbvsmfy6eLB7LLb28Q0cV5+qDwM0JEJHpQLs1D/0DrWTtyrsHatxI2pV3DkYOUsnQkncFAjcJCNxsBa4NFBWFrggcpwthFbu5qQQ6uKkvBfBv/Au6/sGvadPfpaMwoN2jEz3fWf82PT6O4pxv+3p486ZLnhK64+iWWs70qpYBBG5OgIhMB9qtedgf6MXFK/TuwR1aXH2HDrhs5zVafedT2ri0RJ8emGW+qeRuVcmdFbid175Jr36qxO/Re8PeOhbAJ9Q49+rZ9ZocELh0uK4QuGqRzwHPE10QuJu+R54fku+poWOS9O6qIeeAC492aZN73MIjNX+XfHeTjgZG3DT7N+mD9evkf/ABeR4/RaO8HlQI3JwAEZkOtFvzsD/QS4uLdOXVV7WgcU+cETg1vvWsSWughE0KHJ9KfUefet0Zit+wB++90/XkAa9JAYFLh+vaJoFrI7Lt8gS//7YL3Lm4dXOY9LdsIHBzAkRkOtBuzcP+QLOMLS4umN43PjAleuBYxLiHzQqcvgYuXpaHPN+uZwWOe+C4nHvj5AGvSQGBS4fr2kSBA9PRGYGrEAjcnAARmQ60W/Mo+gPdtYDApTNtXfl1b9y4AYFrGHUJnPx+tSWQB26OgIhMB9qteUDgZidwnAfO7Qc6FYjN+WZzvPG0u/4UrXsfqh+XgIL9Ylf6lCVwRX/MIHDNpC6Bk8h9vslA4OYEiMh0oN2aR5poMG058Mp63r17N3fwA9/feOONSvbLtHaVdU0+VUHmfONp33F07rdrrkfHY/K9jQMCB5JA4LKBwM0JVRzw5wG0W/NIEw2mLQdeWU95cJ0Usxa4vPCTGgr6GwQOnKFJAsfpQbzd9JsQ9rdv0qbvqf294A5fAhC4OaGKA/48gHZrHlY0ePj110f02mv/t74eJO3A20RkPeXBdVK0ReCmoQqBu3DhwpmHrdu4f//+cBkIXDNpksBxIl9fCVxwdETbm9fJ3X2s7yqNbt6kkJ+gofadm8ER3Yrn8XK+mudyjrgKk/lC4OaEKg748wDarXlY0fjqq6/ok0++oiee+K/1j+/ZA29EQRAR8WOefN+UqPE0TLamaOwzOstG/kDIg+ukqFrg5OvVGfzeZFnRYOSPWZrEQeCaT5MEzjwR44g2d3fJ8/gyAc7/ZgRuNzA9cLtH4XAe54fzVHmZOd/SgMDNCVUc8OcBtFvzSPbA8W/uH/7wmppK6YGLXPL9AfmOGoZq6KqhsjTurQuUzEWxsblK8AaDZgnc8vJy6h1yVQucRNa1SqrogbMxTt44IHDNpEkC11QgcHNCFQf8eQDt1jysaNgD7R/+8CKlChxFWth8d11JnE+O45DDIufzeHQqcKrc951GCRyLBgSuGLauaT9myV44OQ8C10wgcNlA4OaEKg748wDarXkkBY7H/+7v/rOWr7YceGU95cGVe9+sbEiJm7nA7Qc6ZUgVslulwHGwxMneNw4IXDOpS+Dk/tCWQB64OaKKA/48gHZrHmmiwfCBqQ3IeiYPrEl5a6LAcdoQ4277tO7t0kkYUHgSUnDt27TnB/q6w4Eqm+aOvKoFblxA4JpJXQInkft8k4HAzQlVHPDnAbRb85j1xfZlRJJkubzYXp72m7XA6US+vkkR4oYhOa6r876Fbl/HuuvRnucrqSt+8TYEDiSBwGUDgZsTqjjgzwNot+aRJhpMWw68sp7y4DopZi1wVQKBA0maJHCbriPywO1T6G/OLP+bBQI3J1RxwJ8H0G7NI000mLQDbxOR9ZQH10kBgRuPrWvRHzMIXDNpksBxnjfWtECJ3Em0rRP77vqByf8WHtGRKuM0Itc/+DU9DgM1fly4B3oaIHBzQhUH/HkA7dY80kSDOXvgzZ8HTqcWiU4fEVU18gdCHlwnRdUCJ1+vzigjD9xLL71E169fLxTcnhC45tEkgdN53vhSgSOPPGeTfM+hXc/0wLG4bd7crDX/mwUCNydUccCfB9BuzSOfwPFfyy75rvorWQmc60cU+i6teSG56q9ozgvHD1/nsx/rjk+D0NESF6qCKOD8cT45ah3X9XVZmch6yoPrpKhS4Piutlly3h44i2yzrODXPTg4gMA1jCYJXFOBwM0JVRzw54G6223WvSCzjjwSkVfg8uaB42HkPq3KXHKiOOFvoMrUtKckL8quUiGS9XzxxRdHeoSyoiqBmzU//OEPZVEtsMBB3poHBC4bCNyc0MUDfh3U3W4QuGxbyi9wzSRZz9u3b4+0QVbwPln3flkHLHBHR+kPDK8SCFwzqUvg5PerLYE8cHNEFw/4dVB3ux0e3qHXLi3Su7HIPbu4SAu9V898+RZV2ZV3T0WPp3uv3jn7Jd15jS6p8sOUL2+TY94Ejrl7927uqPIU6qwp6xRqUSBwzaQugZPI72eTgcDNCV084NdB3e1meuC2aGH1XTXOcraqZY6lbnFhQYubFrZejxYWruh5LGqrsdAtLpq8YYd3NmKB29Hrcpldd+HKu3RwOPqlbkLMo8DJNpgUXRY49MCBJBC4bCBwc0IXD/h1UHe72VOoiwssbndoUYjcYu/VYY8bC5odaoHbelZLmu51i3vg7mxc0utuPasEsHdFry+/zE0KCNzk6LLAoQcOJIHAZQOBmxO6eMCvg7rbzQqcPnW6sDA8lbq4aHrbzLg5hcrLJAVu57VLIwK3tbqo101+efW2Dpp5rR0EbnJA4MoHAtdMmiRwN/zf0OOTgLzfeDqhbxjcJH93l/wb/4I+cG+Q6/yA/M0X6eg4oJs3fQpuRXScyAfH+eN4ec4TVyYQuDmhiwf8Oqi73azA6R4zPhUan+q0p1D59Oc4gePlJp1CXbjyqt4Gr2O327QoU+A4HQinAOG7SifB+eH4btUg5XZTTkVi71YtC1lP2QaTAgJXPhC4ZtIkgdMpiTyHrvs+bW9v62PK5s2Qdjkn3M2bw/xwoZq/6V6nH/zmMXmbN8n1d5XUDfR8Xj4o+RIBCNyc0JwDPu8yRjKaDouSrW8esSgD3IWa3c5FBM6P04K4bqTThwRK1qKBH0875HCaEC6LeHwwnMdDTiWy7jhGAtWQl/edyTKYB1lP2QaTAgJXPhC4ZtIkgcuLfipD9iGsNKzA/fSnP9XHhCIBgWsRzTngt0vguEdrMKhP4OQPdtviPHA2/DztnF/gzJMX+K9nLWyc503nf7MCZ3K8aYFTr7vun2hJW3e4PBa4tTXyPE+Vc3LfMLM3Lw+ynrINJwUErnwgcM2kjQJXNyxw3/ve90ZyReYJCFyLaMoBX5/Ka4G8WVjeWOLqwh48+Iv529/eF3Obj60/p7yQ8pEVfFApU+CaiqynbIdJAYErHwhcM6lL4OR3rC1h88B9/PHHU5254f2+vl82cC6acsCHwE2Gv1jM48cn9Nprf5lLaJqErT8EbjyynrIdJgUErnwgcM2kLoGTyO9nk+mcwF29ehWhQlLWAV++zrxFHsE4D/bg8eWXRL/85f+sxqp9vbKx9a9a4OS6bYskct6kgMCVDwSumUDgsumkwMmKzmNIyjrgy9eZt8gjGOeBX4P58suv6T/8h7cpTeD4Oix9Z2Tkk+v4cdm6vmA/ivg5n/7p0I/H/ShlS+Vj61+1wKVhX7vpyHrKdpgUELjygcA1kyYJ3KbvkReGsvgM+9vbw2NsdGzSh1QNBK6jISnrgC9fZ94ij2CcB34N5quvvqJbt/6e0gSOIpdcJWZ8UT5fdM9LsNQ5bqREzdfT/OB2nscCx2W+Erg6sPUfJ3BLS0sjZTYgcNkBgSsfCFwzaZLARWr/GGzfpKNom3aPTija5jxwR3SipjkvHA93/U1a/+DX9Pg4IPc3R7Qb3CTPU39k+0r89m/RzU2XwuCWzhEXqmVtHrnHx9nHvHF0WOC2aFVfb7VKWymVLx5me6PlzQxJWQd83ja3Q29jh3Y2+HFOvZHXnhS9XG3Ij5LaauRzPPMIxnng12CMwP2SUgWuwdj6jxM4ew3kwcHByLyyBS6KWFojCoJ0eZX53fhu1dHXH7/+tMh6ynaYFBC48oHANZMmCRz3wDmuT4ESt/BkQMGLL5Lrhjr3m6vKd1nU1B/UrpIyZzNQvnaTbriOkrVjCt2bOlec73k6b9x1d5c+UEObR+48aUe6K3Bbq1ouWBhWt+JKq7LVVZYOHp7K3UbP/KjwcryOWaZHGztqnZ0NvQ27DIuF3WZyPfN6eeSknpCUdcDnbZ8VuFVVtjNsC/36cVuktZVZ5nR52eZmXpx4Vq2vk9eqaIrMJX/g7fspE34NhgXuww//X+qSwHHvW60CF6i/hF2T640PssN8butP0brHqUIiPe7x6eWBma9W0styDjn+y9muXyaynrIdJgUErnwgcM2kSQIn2VZCln2kqp7OChxLhv2xMJLxSIsFS5kVBysXyeWsuPH6LClW4JI9cGnrbc2hwPH75eHW6mlb697OFIFb6G3QziPTdsnlZZub+p/2wDVN4LguNpedfeoBt21ZIV+vjcFIgUvKmw0pcSxwsj3SoojA6WS9Tnia841Fjf9SDjlprwrudYtcWnf5VAdfNxgn7I2vM7Trl4msp2y/SZEUOAQCYeL4+PjMdyoP5xG4ptBZgeMfCN0zNhSwRxME7vQ0azGBO3t6lsWknNO15w8J7+RlwNu2bWNErBcPY0mOw5adtpVpV257ufwkgTPbWqSthjz6iXuIrIBYiSsTfo02Y+svBU7K2ziBK7MHLjfc25byGK2qkPWU+9ikQA9c+aAHrpnk7YHj5SBwHRO4eQ9JWQd8+TrzFnkE4zzwa7QZW38pcHliksCx8PGPrBW/NNrSdrKesh0mBQSufC5cuECPHz+WxWDGQOCy6ZzA2QObrGyb4jzYx2RIyjrgy7q2Lc4DP+pJUla7WvLUMfL5kVAmfcjZci7jR0SlX3RvL+qvElt/CNx4ZD1lO0wKCFz5sLyxxIFmAYHLppMCl/wA+A3+0z/9kx5vyweTrCf/EBaJOgTODtvWroytq2y3rOD2q7JdLXnaUud2YxFL5IHjcZ0yxOGL7iNdzmcF9UX7StzW/JDCeH66IpVDsn3lASMrJglcktJPodaMrKdsh0kBgSsf9MA1EwhcNp0XuI2N/53+1b/67/R4Wz6YZD2L/hDWJXD8qKe2tStj61q0XZsocPpie93bFg3lzVx0b3rh9JyBb/LDcZJfnQ9OCVy2I02Nrb/dD4sGBG5yQODKBz1wzaRJAhceHak/fPfp+gf/QDc3fdo9CtX4r+k3fkDBdkQnYUBHJ9nHrrLpvMD96lf/I/3iF/9Mj6d9ME0kWc+iomF/CCVlHfBt3fhRT21rV8bWtWi7Nkngmoytv2y/PPHRRx9B4DJingRufX195FF2VQSfln/uuedGypsQR0oc5pUmCdz2TU68e0SB+jy2N6+Tu/tYj3N+N87nxvngZuBv3Re4//gf1+nTT5/U4/KD8dccciLOWs89E745DUU84NNOZhln7Ula09cWmUcTcS9H1STrWVQ06hS4ce3K6ObTTwEwp/psuyZP880CW9ei7QqBy0eyfYtG+T1wATnhgELfJV8NB1Ggc73pp1isP6V7IjnfG3/39WnnOP+byRX3NK17e/oxZJwLLke1ciPrKfe1STFvAiff/7wFBC6bOgSuqXRe4Ph5kr/85V/qcfnBrKkD9RpfK0QscGaaD+R8AOfTUIw+mJM5bcWPJKr6InAmWc9xosF/NcoyjvoE7uux7cpYgWNP0+0Zt2vWhfZVY+s6rl3HBQQuH9O2L0fpAqdkjL/fHn+Hg0AJnK+Fbpj/TS+ihC1Q045LHv/h5nD+N5Mrzo9lbzDgRL9nN30eZD1lO0wKCNx8BQQuGwhchwXu1q3/T4nGv9bj5/lgYp+rhWQ9034IbR4tWc5Rl8DxkwLKaNe6sXVNa9fFxfgJECltC4HLx6T2zYrSBS6GJa1JyHrKdpgUcydw7z1PK8tL9LAheSDrDghcNucVONnmbQk+Vs6BwP2S/v7vL+vxZHmTSdYz7YcwmQxVzqtT4NrWroyta1q7QuDOz6T2zYqqBK5pyHrKdpgUcytwWzy8REtLi/Tyxw/p4PAzWu69TB8/PKDlS2r4QA2X1Hf3CTP+5MoyXXn7Pn32+pNqPfW9vvw23X94h5YXF+iyKmchvLBsng5y/2HxH7+6AgKXzXkFTsLt3hY6L3C/+90/ked9oMfb8sEk6yl/CJPy9v+z93ZNcltnnme9k/ogszMfQCJlSu7dm73e2Xm5FEmJVFWRNuW7idmZ2OmIFiVK6tlwAskbT/ut1bYokeLemABSdmx7YiRbJC2RV9NiIvNOipBUZBUlOSQ3WZX57HkO8mSiTgFIIAsvB4n/j/Ew8Zo4OHkS+NUB8kGUbJQlcPxat3plVFn1eg3Lm4rwlwICl464+k0TWQROX7duEUaflxRNF7idh+/S8Yu3aXvnIX1v5Wl65e1X6JXbD+jqC6v09v1tev3ECi2fensscCvLp+R0rrt3z6yOvtsn5TQInNlA4KYz9wIXJm66aYTLqZ8IdcmoSuCmTTMVVVa9XuskcOo5neE8cK61QS6nCuH7vPi+ztGrut8wuOfQlfch7ssflzNx9ZsmsghcFGnqzgT0cur1kBQQuInA3X39e7J37cFO8D2VPXCyp+3hWOBY2ibTgx64fcPie67WNzEgcNMpXuD64njZopZzT5tePRA4AwmXM+uJEAKXjCpr1no1SeCoZwW/pOQb7TmvGwVSJ/O98U33xL+sDnLCDUcCJ3PHieBfYcof5ozWy5tZ65cDAjc9GidwEXXQpIDATad4gSPqcK63wVDmgOt1LlPbvUdtyyHHcWnX9+iy5+urlAIEzkDC5cx6IoTAJaPKmrVejRK4WXCDB7bLnrsCmbV+OfIWOE4J4vsH3y9dHfTI84InWrD05km4nD/4wQ/G39m0AYFrTkDgplOmwLlC3LzLl2ViX3/XIa8zoLbzG3Ks4A/nsmm0wAUZ7ZMJz5cpMHrFXHoKEy5n1hOhKQLHPUH8CKdogpQiwSD3DHHalqCHKMx4mRxRZc1ar7UXuJKYtX458hc4j1whYZw+REpYT4z7vSC3m2hrG44v04Nw/rcgP5w/GbY49Y0vU4pYHqcSccnjZ5PlQLict27dOlAP04LbXN7tzgR0geNxfd/rFofhwoUL8vm/TcUkgTOVRgvcLHngZCLfHl++Wg/ymom5Pfk++SWnDZdTHazThikCJ2uiN6krriOZDnkka4y8tMd1yNNkDq4ggru1Rvnjcs7fosqq11uaKLJeFfqXrI7BGCNwVvD4MPlcWL7M3PLFuCPaVpATTl5+lqIW9Myp4aCdCqGTbdOnDf4LO0XZ0hAuJwRuQpTAKf7pn3z68MO7obn14rPPPsv0neB9P3/+PAQuBRC4RgpcIAby6QBK4OTwfnGQwz2Wu6CHqNc6Mb6/aJ/AjdbNg3A59csnaUMnrwN+VN1GTQtkd/J8Tq4jrkd5M708KQZJfuMETi5bkMDpdZUldPKqV0VUXdYJVf4sJysVXL9pJCmtwOn0ej0hZLO1J778PL1k6QiXEwI3IUngfvKTl+i//JfgyS91BAKXHQjcdBoscOZymHIWLRpRZYuaZiqHKSsfUHUOW6/6r18PUz4TUOXPcrJSUbTAmUK4nBC4CUkC9/77/ye9917w7GWdfr8feV9jn5++ER4Xy/E439uYpp3lCQQuOxC46bDAzcrPf/5zcwWu7qHI8qXnKEPg6h5M1noNX0JVsiWGhIBl/wqEP4tZBI57krgfKU1fUs/ly4buJPVIwcxavxxZBE5ft26hgMBNSBK43/72ppC4/xqaO4FFzRLt27J98h2HNl1XSBpfPrdoINqTZ9vy/sau68nppgnc1tbWgWkQOAhcGpTAcfvS21BS8L4bLXBRxE03jXA54770cVGGwOlETTMVVdas9aoEjoWLxU3JWyBy2Uj6LNLUpfrxjcr1Jn8Awvdw8UPbgwF54z7/gjJYdvQcWv7ByLBH6+JkxvNzuid/H7PWL0cWgYsiTd2ZQLicELgJyQL3B/rjH0+H5k4IC9w5y6cbLTEsXh1+vq3D9zFa8ocpfE+klDmrK39RWCZxAre2tgaBiwECNx0InIGEyxn1pU+KvAVu0tsUEFWHUdNMRZU1a72Ge+AUet2kJemzSFOXsldt9IMalaRX3rspc8EF0ibvKVT53+S8jeCHNsLaOFzxHml68LIya/1yQODSRRMFznV/T3/4w4XQ3HoRJXAsb3rvuwoInFkCx2lD+Mh0uS3+GPAuk2eJ4+v5/43O/eZ/ypxwnAvu3u5Azm9tfJ8+cT3qiTL133uP2uK43G67Yn72Mk4DAmcg4XLqX/ppkbfAqd6mpEt8UdNMRZU1a71GCdysJH0WdarLKGatXw4IXLpoosB9/vmX1OlcD82tF1ECF5Y3CNxBzBG4Pu3uBp8Dfx78hzLngLvXapMn88EJuet3yLr3WJajz3ni3uNkv47MGcdPcXCcFllu/k9yaLTAZcsDN0ovMuOv2LIQLqf+pZ8WcQI3O7MJXFBL0XWl13vwa9Oo/HpBD5JMKRLxVuoXq1lQZc1ar2GBU3UxHB7+HjidqLqsE7PWL0feAueJA61rebIN8a9PfdeidYfThAx5ZnCDe88jfyi+2xvPTPLFFUy4nBC4CUkCV3d0gdN733SJg8CZJHDmEidwervi+Pzzz8fz50Lg3PUgtYW8yZvziPB9QcQvrbEwtNZP0DrP41QjMjGtvCAVXL4KFs4sEdMIlzPriTBvgVMfvkKvw7hpwf1ZLVk/QZLeUZoQUZ8ybYh4Vc/nVPWnJI7nB+sFKUY4hQvPkjIn1lGfF+eXC29DvkeU6YVQZc1ar0rgou6BUyfULBFHVF3WiVnrlyNvgeNHjrkuPzpMvPpD+Zezy/dC8TbEPLkpIXCcD86XsseXpoPkvkUSLicEboIubPp4ndEFblpA4CBwaYgTuCeeeGLeBW7UR5QhD5xah4fddZYHvv8oey/QNMLlzPKl58hb4HSi6jZq2jhX3uherHDOPJXvjRP9BnI2EjIWNwrqXAbX7ai3bp0FrsXLy1vyg89LfAZqvUAG1ScUjypr1nrldqXXqy63eRBVl3VClV+1w6yRq8AZSricELgJurDp43UGApedsgROr/u6BB8r4wSOIyxx+rw5EDhzCZczy5eeQ50I82O2HjhTUWXNWq9RAlcEdarLKFT59fpLEzdv3oTApQgI3H5S54Hj8b4X28PqFyhLELjslCVwOlz/dSFJ4Dj0njcVELgCCZczy5eeowiBm+UeOFNRZc1arxC4dKjy6/WXJrIInL5u3UIBgZugC5s+HgeLmccpdHy+/M153lxy/AH5Mr2IS74vYsBpRGza63JuuNE6Ht8XaZPj8D2SPlmtH9Ow55Ftu9Tr2GS7XX1TMwOByw4EbjrTBC4uIHAFEi5nli89R5zAzX7An8gbE1WHUdNMRZU1a72GBW5SH7P9iCGJNHUZXFYO7ivke/5kvje+rSuUB46nT1eh/FHl1+svTWQRuCjS1J0JhMsJgZugC5s+HkeWPHA9+69ojwXOtmnT6oplbSFyfA+kTTdu3BDL2+QKceN8cXn2yOm3CqQNCNx0IHAQOKMIlzOraKgvvs6sB3z9Hq+oOoyaZiqqrFnrVQmcCT9iCPLAjZ4bO+Sb9NeD3G4yDxzflxn82KMKVPn1+ksTELh0Ma0N1RVd2PTxOjNLrzG3DQjcdMoQOM7v5tzbpV7C59Hp9GTqEMf39VmFAYEzkHA59b/I0oZOXgf8qDqMmmYqXFa9rrJEGJa5NMKhk/RZ1Kkuo1Dl1w8YaaKJAjcLv/jFLxLbUF3RhU0frzPqEmrawCVUswSuc7lNrhA4y3GkqLlWm9qXuZe3LfO7Oa4np3Py3mH/Mu3679F77XO0y729ly+Ty+G0aJe7f3Ok0QI3rY+CL1VVQbicesVPCz4J6vvJ5HXAj3rvqGkydYhWwUF9xtU6pwdp6RNzJ1xHWYPXy4OkzyKqLuuEKr9ed2kif4EbpZyJaFecI47h58oGBO3P58vTKcpwGPRy6ifvpPj7v//7Rgmc3kbqFgrcA5cdkwSO77G03F2ZlNey7klxkwLntOX9lTJhr5jOPXD8NJy2ED5O3qsEzvGEwAnRy/vIogTuV7/61YGrPNNiLgQuuNw0yQPHKUM4XQWjDvw8j+85UqlFiiZcTv2LPS1METhO8cHVxSk++H4svqIX1GeQUkSm/ginFiFedpQKRNQ3P/ZJfj6jZfMiXEdZAwI3HVV+ve7SRL4C15M3sDMsaxZfWm6pPG/igGs9K3+NyPdF+UNXzueb2nlZmQLHG4p2aYm/rLn95ntJRP+M9XpIiqYJ3LwAgctOGQI3y3omwiKnt6Fpwftfa4GTz5PkZL6cz03mGAtyvoUFjuWN50PgAqLeO2pa8AD14F4sNZyUGy6QvVH99loyz56e4y0PwnWUNcoSuLrHrPuRr8CRfNpC0M5GAsfPfx1yO/NFWxwl9Z0qcPzkhv1ve1j0cur1kBQQuHoCgctOGQKn90o1LWotcKYSLqf+xZ4WpgicqYTrKGuUJXB1RpVfr7s0kbvAGYpeTr0ekgICF42e8y0rnEtuSP1U7W8WIHDZKVrg4jh69KiIBfrLX/6izyocTr773XffySgDCFwBhMupf7GnBQQumXAdZQ0I3HRU+fW6SxNZBE5ft24RJjxdpajRQ82HwEXg2XTOasn0IR6LnO+S5e+RY9nUs23yhAT5A0emC+Fhvpnccrtymm37wXyx3mAYXF4vAghcdpoocIySuDIwVuDqHgp9+rQoQ+DqHnUXOPlM2NHr/ovLkzFOKVL0zfhRqPLrdZcmsghcFGnqzgT0cobrYHl5+YC8hR/3A4E7iG0L+RoMaXNzQ973yJfHLX8wFjge3idwLG+inSmBs7sD6gqB45vO+X2KAAKXnSoEjuXtL39hrVmQElcFEDjtAKmIm24a4XLqX+xpUYbA6URNM5VwHWUNowRudH/muvoBTjBn/w9EUshQ3qjy63WXJiBwQYQlTn9WIwSunkDgslOFwDEscZNk7eUDgfsq+kAeN900wuXUv9jTAgKXTLiOsoZJAsc//hiK1w3+AUirJ39Rue8HIuiBMxa9nHo9cOiXTlVA4OoJBC47VQkck/f7mUqtBY57KThPVBIHL1MVT7ic+hd7WpgicNnzwI3gVC6jXwEXQbiOsoYpAmcyqvx63aWJvAVO5XrzrA1y/aF8ziVfOgsus7XkY8dYdMUSZHk9IbxCfDeeoQ3Hl79IVVLMbVk+mky0TV7/sOjl1OshKZomcPr+1y0UELjsNFXg0u53HtRa4JRKSNmgUZ4yKRAjqesFaSxa6ydGqSySZS8v9HJmIeqJAUxeB/yoskVNk6lD3CCRKtccnwBZ4KScjeqYn+U5qXOWtuDSoKxpUefr4/V7clheOuRph0grElXWtPABNQ+SPovDlM8EDlN+rt/8BC5NHji1TJBCpDcUgjZKL6IETib3HXKqGyvo7cwhJ5xeR/rJOymaJnDzAgQuO2lFBgI3O7UWOM4Dp3KNBXnIJrnKmKCHTkkG54krX+D0TOzTwiiBS5kHLrhvqyfrN5C01ig3HEtg8Nmo/Hy8DL/XrAqnyqrX27Tg+ouq11lI+iyi6rJOzFq/HFy/+Qmcuejl1E/eSQGBqycQuOykFRkI3OzUWuBMJVzOLF96DlMEzlRUWbPWKwQuHbPWLwcEbnpA4A7S92yyLEcmZeY/Eh3HOZATzhPtiqfF3Rfq2faBdfIEApedtCIDgZsdCFwBhMuZ5UvPAYFLRpU1a71C4NIxa/1yNFXguD1kCQjcfjiJr7z87U4EzrVa5PoDsi2XHN+nTbdLe0OPnD0xzbbI6Q7I42W6Xdq8cY9uWJtyWlFA4LKTVmQgcLMDgSuAcDmzfOk5IHDJqLJmrVeuv6h6nYWkzyJNXY5/Ydpz5cOV1TQ+efVcvs9r1gvMh2fW+uVIK3BplqkTej1MC+6FS2pDdUUXNn08DtkD5wpxG30H5H2LA4cs2w+S9lr8ODQ3SNor2o4rBI+nhfPAObYth4sCApedtCIDgZsdCFwBhMupf+n1JJ96qgEIXDKqrHq9Li4uHqjXcN2aJHB8433wC8nJPZkscKw1VeV/U8TVr16vHDs7Owfabho503ukmhrzhi5s+vgs9Hr8C+PixCwtELjspBUZCNzsQOAKIFxO/UuvnwQhcNlQZdXrVa9b/XFNXH9R9ToLSZ9FneoyiqT6XVpaim23HGkFrgq47HwybfIJtWh0YdPH6wwELjtpRQYCNzsQuAIIlzPqSz/tJBglGknSkIWoOoyaZiqqrFH1Gu6Fg8DNRlL9hgVO733jgMA1G13YeFxvI3ULBQQuO2lFBgI3O7UWOE5voZJ0BjnKRvnJeJ5M4Mu5KyY5y+RyFKS0kDni5OxRqoxR6pE8CJcz6ksfJ28cpgicuz5KzcIpQjgNyDgfHD/+KagvmbYllCtOpmvhxCLrJ4L657rP+X4uVdaoelV1q8sbBwQuHdPql0UoSt44IHDNJkrg5gUIXHbSigwEbnZqLXAtlgiWMFLyMMq8PhIMOV/lLpM5ywIBUTnjZMibZoOcZnkRLmfcl950gVsf5WyTdSvraT3I5cbjHLJeg9xu4zofJUrmaVKoR3KcJ6qscfUaF2UKXN2DiatfFiF9mgoIXLPRhU0frzMQuOykFRkI3OzUWuBMJVzOLF96DlMETpGvfh0eVdas9VqmwNWZWeuXw2SBY/jXn6A4dGHTx+OYngeuHzx9w+vvm1omELjspBUZCNzsQOAKIFzOLF96DtMEzjRUWbPWKwQuHbPWLwcErtnowqaPx5GURmQ4DJ5ra59zabA3lAl7LbdLA98jq7sn04eU0eIgcNlJKzIQuNkxVuDqHoosX3qOMgSu7sFkrVeTBE7meot5rFuaHk91z6bbS7N0NmatXw4IXLPRhU0fjyNJ4FQeuODZttwLZ5M/FELHIucH+d/KaHEQuOykFRkI3OwYK3BRxE03jXA5lZBlDZ0kachCVB1GTTMVJQqzRh4kfRZp6rJnbdDG+jrxw9n5/kGZ/210v6B83mxvKB/ILpP7usH9mTwcPINW/eiGb0C0xIlvGPwgp7UhH9iulpv1pKbKr9db2oDANRdd2PTxOgOBy05akYHAzY6RAjdP6F/stKEzb/VyWPT6Sht50ITPQq+3tAGBay66sOnjdQYCl520IgOBmx3jBA5EMw/SwHna5oF5+CyaCASuWHRh43FdbOoWCghcdtKKDARudiBwEXCKD9OouzSwvHHvjMk9NGkx+bPgttvkk0YSELhiiRK4eQECl520IgOBmx3zTKViAnkzr1pMloY0qKckQOCKZTBYoKUl89qvCUDgikUXNn28zkDgspNWZCBws4Mj/QGCpyRMXs3AZGlIA3rgikc96oolrsknjjggcMWiC5s+Hke/36dezHEh+IWqU/lxAwKXnbQiA4GbHXMMxRBMlDfGVGlICwSueFjguO2iBy4aCFyx6MKmj8fR9zzq+a7M7ea7Ynjo01AM25Yrk/uy3Nn+gAY9j7qDIXnnvk+bN+6RbbtyvAwgcNlJKzIQuNnBkT4CE0/QJpYpK/OwD4zJ+8GSMg+SXAQQuGLRhU0fj4N72TzXIddxgtQ6TotuOJwOxxcCF/TA+QN/nPutL17t7kDM75JfUluHwGUnrchA4GYHAheBiSdoE8uUlXnYB8bk/YDAxQOBKxZd2PTxOgOBy05akYHAzQ4ELgITT9Amlikr87APjMn7AYGLBwJXLLqw6eN1BgKXnbQiA4GbHQhcBCaeoE0sU1bmYR8Yk/cDAhcPBK5YdGHjcV1s6hYKCFx20ooMBG52IHARmHiCNrFMWZmHfWBM3g8IXDwQuGKJErh5AQKXnbQiA4GbHQhcBCaeoE0sU1bmYR8Yk/cDAhcPBK5YdGHTx+sMBC47aUWmCIHjY7TpkQcQuAjyqtw8MbFMWZmHfWBM3g8IXDwQuGLRhU0frzMQuOxUKXBVkma/f/WrX+mTZgICF4GJJ2gTy5SVedgHxuT9gMDFA4ErFl3Y9PE4ZB64Ua43zgHneT2ZA84fDOX0oWfLHHCc921IfZkDbs8PcsKV1dIhcNlJIzIMBG52IHARmHiCNrFMWZmHfWBM3g8IXDwQuGLRhU0fj2MscN5ASJtDlu3LXHAW54FjgevbMsmvZ28S5+31hBS5nBPO7Yrxcto6BC47aUSGgcDNDgQuAhNP0CaWKSvzsA+MyfsBgYsHAlcsurDp43Xm3LlzMwUEbjoQuNmBwEVg4gnaxDJlZR72gTF5PyBw8UDgikUXNn28zjx8+PBAL9u0uHXrFgQuBRC42YHARWDiCdrEMmVlHvaBMXk/IHDxQOCKRRc2Htelpm6hgMBlJ43IMEePHqW//OUv+uTakna/8wACF4GJJ2gTy5SVedgHxuT9gMDFA4ErliiBmxcgcNlJKzIsbyxx88K0/X7iiSdoYWGBvvvuO31WZiBwEZh4gjaxTFmZh31gTN4PCFw8ELhi0YVNH68zELjsTBMZRRMFjuUNAlcQbMemYbI0pGUe9oExeT+47Tb5pJEEBK5YdGHTx+PwbIusVivyF6WebZM/4PQi5aUMiQICl51pIqOAwM2OeaZSMSbKG2OyNKRlHvaBMXk/+ISxtLSkTwYEgSsaXdj08Xj6Mvcb9T1yfJ/cze/LPG+cCy4QOIes7oC6nk2O4waix8uKab7nyXV91ya32w1yxHGukZyBwGVnmsgoIHCzY6atVIyJEmeyNKRlHvaBMXk/0AMXDwSuWHRh08fjCQSOZc2yfPJbFrUsS8haixzrHG22bghfs2nTaonlhpM8cGLZbound+mGeLVtn2whdQX4GwQuI+qHCUk/TuA0Kw8ePDhQb3UPFjh9WlxwHRwG80zFACBwxTAP+8CYvB/ogYsHAlcsurDp43UGApedab8uhcDNocCdPXsWIULHZGlIyzzsA5O0H/rnWHYsLi7SmTNnDkwvK0z+AQUErlh0YdPH6wwELhvZeuCu04tHV2XHyZcPdg7Uox7XXzxKa6d/fWB6UvDyX9zfoWePrNKTf/MhbT0Qn+f1F+no2jL9zYdb9ODhwXWi4uja8anLNl7g9J1sYugkSUNdmId9YJL2Y5YD/TwFBK656MKmj9eZWb7XELh0AsfytvzcWwfqLy5ePLomls8icNfl8krglo5dpA+3HkgRPHH8SQhcnkDggtBJkmxvwsYAAFRWSURBVIa6MA/7wCTtxywH+nkKCFxzYWHjXpTd3V35+rOf/UxfpLbM8r0OC9z29vaB+U2Kzc1Nevz48b46VQK3evxlurkV1M/dN56ltZVjtP0w6JXbunqGjq4u09ZHrwv5WqGt7YcTgRv1oPG0YDt3aeWUELV3XqDnfv0F3d9R298vcKdWV+n433xIR0+8Sr+++D0pcB+9/iz9+ov7tHb8b+jDK88HvXRXz45f1fvx9u7vcNnW6NalZ+jI6im69uITcr0jYt6XD+4IgXuTPvzyAT2MqAc95lrg7lw6Rgunrx3Y6Uxx7bQ8mPDwtdPBa9R8jmv6vKi4c4lOX9u//rFLdw4ud8jQSZKGujAP+8Ak7UdwoL82blMLp6+KaQc/XxnXnpeXPMPzj4vxY69+fHDZUFx7fpEWxfsG49docbStnbjthILfP7Y8OQQELh7ePredeQ5mZWVFvq6KE+W8kIfAra2tjc9FTYtEgVt+jt76MrgXjnvFeHxHyNgbzx6RMnZ09fhI6NYiBU6KmhiWx1sx/cNXT0gZ2xlvf7/AKfE6/uqHcpgF7urZo6Nj9nP061+fDnrl3n0xELeP/5ZOjQWOe+Du0t9+/wj9+vQarS2PjvNivbVRWbgHTpVFrwc95lfgpFgdo2OiIsbCJKadPi2kboFfudJOS+m6dCyoRF6O1wmWOUaX7nwlhYvfQy2zcPoSLRy7RHfEevKV35PffyRmLHlyudEyvN3wtLHA3Rm9jxofl2203a/uTLbJX1oliimFVEcdHOvMPOwDk7QfSuBYwliUFhdP01Ux7bXji1K0Tl19KKWN28LDjy8FQiU/8ztjEeN1w8s/FKJ36tQxsd4Cvfrxx+PlPr50XL5/uN3w+/G8Vz8O1gva3ym6NHo/uV0xfYmXO3U1lfRlCQhcPFVvv2j0S6b6eJ2ZVeBYZh894tNsEDz805/+VE7fzvm7x7F6+i3aevAxrSxOzj0LGS5Ppo3rZ48E96tdP0tPvnwz1AsWHUkCJ0VsdVmWld+TJW5cbjlvv8DJe+CEOEm5G0kT99wtLDxJS9wDJ0RNiRUP8/bluBAqFritB3fob4Uc8mXUO3/77OgS6l1aXQqW+eLtMwkCF5Rz4cm/EcftQOSCsv6anpBluSt74Hga997p9aDH3ArcadX4ZIxOUkKCWI54GkuTkrvwckqgeH3ZMzYSOD6p8jT13qcvXQpESwkc95qc5p6TkBSK8UDeePuBkElxC5cpJHDh7cr1RsIntw+Bm4t9YJL2Y5/ACVGaiJxqo6ciBY571T4W66oeuPDyV6+eDoRs9L7Pi2UWhXxde35JLHdqX7uRYsbrsZyJNnfq6o58/8Vjr47fHwJXDVVvv2h0YdPH4+h7HvWGPg18l/zBkHpi3LIsajk+eecsmQPOspxxDjjbtmQOOF5Hph+xWuMccEUxq8BF9cA92LlOZ9dWaOtPr9EzUlyeoou3btPysYv07tk1Wjl+kVZXjtPJk9whcFK8LsnXK1vcS3V3LGcPrp2lNSE3y8v8nX+KLp0I3j8Qtrv0xjNrkx8FCNE6Irf1HH1PbHtJLPfyzfu0/e5Zeu65Y+I9luR4sIwQKS6bWI6liKfze9x94xk57+XjK7QiZectudypt74U+yTkkSXpyeBy6BFRLn7fYP2tZIGLqLs6B+6B+2oiaRMB+ypB4ALpCtabLnCqR032sB0QuP3ry8u4Ytp4fSlsgczJ9WMEbrxe6BJuUL79PSZxoZMkDXVhHvaBSdqPAz1wog2wfLFocU+c+nxZ2K6GBO7Oa8dHyymBCy0vhCtK4L6685pcfyJhd0aidk3O3ydwi0E5WNzUJVS5jZ3sJ6akgMDFU/X2i0YXNn08Ds7xZrktGvaEuPl75Fi2ELVAzs4JOVtvOTKZr8oBx23ME5LH0zj3m3rleUWRh8DxtEDYxPnnqYvje6TeeGaVlk9dEQIkzj0nVunpV27R2soSPXVx/+vJK1tSot4SIndNiN6tt5+nVfF+9z96XbzvCt3fflf+GGBLSlu0wN3fd7+YELB3XpA9aPwex15+i1aOCQG7P7lfj9+Dl1Prs6jxJcmzR1aD9777hhS4P732rBQ1nn5cvB8vy+/7rJDAY+J1c3PjgMCx6OzsTP/VqakRd6yblshX8aMf/Yh+/vOf65MzYazAFRpSqtKJ1KFjdCn4wPQpoZMkDXVhHvaBSdqPWQ708xRxBzUTqFqgqt5+0ejCpo/XmVm+13ECF0jQSNyOXaQz3CN28gpdF1LGYndLCNTaynG6eOv+WNx4vVPilZdhgZP3d10PeuD48uLZtdVUAvdg5+5I0q7KZVngZE+ceA8WL16WL48++eQSLYnlzhwJfiHK4shlSS9wx/eNR/XAKYFjvv76a/rssy/kMJehLnC5FepzT9sD94Mf/AAClz3uTHr3DszLMUY9f7NuSydJGurCPOwDk7Qfsxzo5ykgcPFUvf2i0YVNH68zs3yvpwkcS9Hq8iI99RT3qF+RksW9bdxLFidw4UuoW0K+9gvcw+AyZsIlVN42v2dwv5gucG8Fl0bFe9++dEIuJ0WOe+C+CsROXl7l+9R4OOoSKk8T8w8KXHwPHPP48YCee+5/l8O8rboAgdPgk2P4A+QK+uabb+RwXT7YcDk//fTTTMHXxKOuiydJQ12Yh31gkvZDffb8Gv5y1wVVfr1dpglutxC4eKreftHowsbj+kmrbqHIS+Di4y4tL/L9cfr0+YhpPXDffkv0i1/8KznMy+t0LttktRx9ciz9TkdeUufXXsSl9c7ly6Qfqfr9vpjWp/fe62lz4oHAaegC98UX92vXtRoup36SmxYQOPNJ2g/12fMXm9tu3VDl19tlmoDAJVP19osmSuDmheIFbr4jjcB99NEzcpiX1/F3d6VwXbYt8r3LwQ9Xzv+vdO43/0S226Vd36Pu7kDOb218nz5xz9N59xO6/AOLnMe7dLnt0j0xv205ZLk+OdZ5cu7t0qDXkdN7nct0z/Wo3eYfz6T/MQwETkMXuA8//D/o/ff/hRyO+mBNJFxOPrHpH1xSQODMJ2k/1GfPB6QPP/zXYshcoYlClT9ru1VtFwIXT9XbLxpd2PTxOnNYgVOiwvz5z38Wx4dv5TAvVxdUWfU/3JKC2wAfL1lU9vb29r1fWOC+++47+sMffiiHo+rksuuQI8TLdcRryxZC51BXvNrdXTHcJZt76PxdMb9LrmVT1+OeN5d2HY864jNw3XvyF868nm9dpvMbG2RZQuSctpzuXb4sJK4jxltien4C98QTT+z7EWM4GiFwv//9hjDzE3JY/2BbLZf0zk631ZLTeu7Beb2enKNNzZ9wObOeCCFw5pO0H+qz5+Pz73+/SVEC13Mt2XZ1uM1y+3TduDbai1xPLc//x6+bDlX+rO1WtV0IXDxVb79odGHTx2Ppe2RbFhncdHIVuJ/85Ef0138dfU4zGVVWfT+TIovA/fGPz8vhOtXJNIGTvzgW8fnnnx+om0YI3O9+9zZ98MF/lMP6BytOddRy+aTWIj5vyVcWuJ4rT4auG0wXI+K1J6fxMkUTLmfWEyEEznyS9kN99t9++0i23WiB4z8uerJdWkrIRm2W2ye3aZ7eGwZ/kHDbXee/PkdtV83j5Ya94DvAW+F2r4ZnRZU/a7tVbRcCF0/V2y8aXdj08Tg82yZfiI537vu057PMuXKY87qpXG+258sccHXiwoULkQJ38+a/o2vX/qUcVt+3OqDKqn/vkyKLwH3wwf8th+tUJ0kCp3rf4iSuEQL33nv/Q3yw/0kO6x9syw1Odq488QWv8oQ3Cnky5OmjVyV1RRMuZ9SJUH2g+nQOCJz5JO2H+uz5gMRtN0ng5B8YwV8YY3njaUFbloonTmBiuiX+SBHGNnStoI3zMmK8J05oLic8FcvzuU2198Oc51T5o9qtartxuZsgcMlUvf2i0YVNH4+DE/l6om37li3auiUvY/Gw5QupE8OblkOeFST5rYrPPvss9jsRFbzv58+fjxS4f/zHl4XE/Vs5zMuG6XR6Yp2hvK+LZba7x/dntcnt7hHvfluIbNt25fS2vN/rWXnvVs93yRHLBEmN9+Q4Jzzey7HOVFn1fU2KNAKnr1OX4GNdnMDp8hZ1zm+IwH1Af/zjSTkcnp4FvnTKolcW4XJGfenjPlAOCJz5JO2H+uwDgfuAogTOZFT5o9ptuO1GHXghcMlUvf2i0YVNH68zeQrcb397k95//7/KYV42jBI4X8jOsN8hq7tLjt0mu+0LARpKOevYm3I6v3e/LeROhOU4Yt2OWM+Ry9riD729gS/fKy9UWfV9VfHll18emJZG4HT0OjEZCJyGLnBff/2N+IvsN3K4Lh9suJz6l17/QPUPFQJnPkn7oT57fuW2WzdU+fV2u7TEuaPi261quxC4eKreftHowqaP15l8Be59+uCD/yCHedm6oMqq7yvHkSOjHHGaxDVV4PT6iYq5Fri6h0L/0usnQf1ECIEzn6T90NtBHYPR222UwOm9cBC4ZKreftHowqaP15k8Bc7zfk9/+MNLcpiXrQuqrPq+KnmDwAX7DIGL+QDjpptGuJxZvvQcEDjzSdqPurTROFT5s7Zb1XYhcPFUvf2i0YVNH68zusCpB9OHY2uLn5QQzI8SOP37Uqdgwq/h0Osh3CmRl8BlzwMX3BfYcsS8HC8j60DgNCBwEDjTSdqPurTROFT5s7Zb1XYhcPFUvf2i0YVNH68zusBx6BIXnhclcFHwsnVBlVX/3idFXgI3Sx44Tk1jWULgdos7JimBe+mll8bnbt5XNTwtGi1wUbnedMLzkQeuWuZhH5ik/dDbaDQ9cZBpRf5aVP76lPPBydQ3+lySvzotElX+rO1WtV0IXDxVb79odGHTx+vMNIEL975xQOAm9ZCHwJmKErjbt2+P9zltDxznCTzsMaHWArfeaolgiePUCsG4TCsySqfAyJQiFMhea5QotWjC5eT9yRIQOPNJ2g+9jUZhWW4gbyxprXV5aYDFLEgRMkkp0nI5VUiQRoRfufluyFQjQboc1dbzRJWfDyx625wWELhkqt5+0ejCpo/HwWlEON/bwOdUIaO8b3wprBU89mjgezKFSM+zye0G+eI4R9wmp9oYeuR0xXJ2kDvuhrVJVuvH8hFJ/B7j10NeRosSOBW6vHHMKnCcGiiqpPw8z/B09dxO/tVqWaiy6vuaFE0ROIbbBwfvsxpOioZfQg0abs/lJy+MBE4OB9IWFjgelKJXQR44vcs0begkSUNdmId9YJL2I67t7sO1xIlpkvdtLHCjnG6yZ9l6hjaEwPH8dZ6mBG59fZQnbtTWc+6RU+XX22PagMDFU/X2i0YXNn08jr4QM4u/Bz2PLH+PfCF0m6Jddwf82CObNsX3JHh+pS1kzKe+bcsHlXv8nZF54iy6IV67Yr4rkwI75DielCf+Y2eQQzqNJIGLilkFjvPhce+863E6EVEfnBeP87qxwPU71O0GOeD4sU9Scp3uvvWLRJVV39ekaJLAqX1O2wPXcIEzl8OUEwJnPkn7cZjP3gQOU34+YUHg4ql6+0WjC5s+nhohdIf0rdwpS+D4vq3unkN7XZvO2115zxc/s5Pzwnmc781lQR2GnttZrsDx0yX0P9rSBATuYEDgDCVcTr3bdFpA4MwnaT/q0kbjUOXX22Wa4HYLgYun6u0XjS5s+nidKUvgTIbLGr7XK23wvV5cHxC4/THXAlf3UGT50nNA4MwnaT/0z7OOwWRttxwQuGSq3n7R6MLG43obqVuE94W/91liHgWO0f9wSwpcQo2PuRa4KOKmm0a4nFlPhBA480naj7q00ThU+bO2W9V2IXDxVL39ookSuHkBAletwNmWQ91uUJeK4Icd/cj7G/lHHvyDEPVosqKAwGnwhx31ATJx000jXM6sJ0IInPkk7Udd2mgcqvxZ261quxC4eKreftHowqaP1xm9raeNeRQ4fR+TIi+BO7+xTj/+5LH80Qr/mMXt7srnv+4OfJn3TU23L/syP5zPP/JwXQhc2WQRuDR54JLgvHDyl3xRCbcOQbicWU+EEDjzSdoPvY1Gk5QHbpIiJGJ24ajyZ223qu1C4OKpevtFY6qwra6u0uPHj/XJpRIlKky644UZqLLq3/ukyEvgTAUCp5FF4Nz1IEWIzPUmXlsyVcgoP9b6CVoXrzyNT4qcc0vNd3vBfCmAPB0CVwrzsA9M0n7obTQKTv3B3fvcJq0W5ybkQYuGo7Y4lH9WcKqQHlmcUoBzvYWWLRJV/qztVrVdCFw8VW+/aEwUuEDeFiBwOaDKqn/vkwICFx8NF7hJHjgWtEDUgpOdPP2N8r+Fc8TxCVHliOP544Spo5xxeREuZ9YTIQTOfJL2I67t7mNKHrjhcNQee2La+nrQKzdqv0Wjyp+13aq2C4GLp+rtF43JAre6Wq3ERYkKox8v1CU/fr7nHj/fU+Z8a5Pb3ZOpVdqeT207yAXX5jxwG8/SvdFzPzlfnHxGqHjltCP8TeRleDr1O3JYvecs31JVVv17nxRpBE5fpy7BxzoInEZ6gZtQ/GktG+FyZj0RQuDMJ2k/4troTHBSz9aot64kVPmztlvVdiFw8VS9/aIxUeCUvNVN4KSACemyurvk2G2y274QoKGUs469Kafz/XX9tpA7zg/nOGLdjljPEcu6Ytng3jtehqdzcEJjvl/MYjmc4b4wVVb9e58UaQROR68Tk4HAacwicKYRLmfWEyEEznyS9qMubTQOVf6s7Va1XQhcPFVvv2hMFDjugeNnlVYpb0yUqDB1Ol6osurf+6RoksDxfmaJuRa4uoci64kQAmc+Sfuhf551DCZru+WAwCVT9faLxkSBY0woV5SoMOr7VgdUWfXvfVI0ReBeeuml8bmb91UNT4u5Fbgo4qabRricWU+EEDjzSdqPurTROFT5s7Zb1XYhcPFUvf2iMUGUojChXHW+14uDCb+mjbwE7rz7CT0eeOR84lB3l+8LvEyO48pn5FqtH5PT3aXLtiun8/2DvYFPXqcn7/27bFvkti+Tf8+he7v7c8kdFiVw4SdUpL2Eyk+oOOwxAQJXAOFyZj0RQuDMR3WBLy4uSmFRr8zDhw+1peuFartZ261quxC4eKreftGYIEpRmFCuKFFh1PetDqiy6t/7pMhL4HoDV+Z4+4FryWfBepcvC0EbkG9dpt/85jfyebGuEDee7u/uBgJ32aOOGOfnyTotvk+wJdfNEwicRhaBk2lBtF8wjH6bun9iCM79plI0FEW4nFlPhBC4esHyxvfYqNDbaN1Q5c/ablXbhcDFU/X2i8YEUYrChHJFiQpTp+OFKqv+vU+KvATOVCBwGlkEbn19PcgDp3K/ieEW5wwZCZxKiKqnDoHAVcM87MM09DYaRZCAmnPchHK79ThH4SitDed/4z9OhirHYY/WXZ/80fwiFUmVP2u7VW0XAhdP1dsvGhNEKQoTyhUlKkya44UpqLLq3/ukgMDFR+MFjk+Crsz/xkPBsMqtxfAra5oucFL4cs79FiZczqwnQghcvdHbaBQ9a4M2OL9by9qfB06McxJfma9w9EdJbyjatCXGhRhx2w2S/BaHKn/WdqvaLgQunqq3XzQmiFIUJpQrSlQY/XjR73TkMz5NRJVV/94nBQQuPhovcKYSLmfWEyEErt7UpY3Gocqftd2qtguBi6fq7ReNCaIUhQnlihIVRj9enLdatN5yZD43vsfrnHze5yDI59YO8sEFud588oQQlal6qqz69z4pIHDxAYEzlHA5s54IIXD1pi5tNA5V/qztVrVdCFw8VW+/aEwQpShMKFeUqDD7jxd9mYC332nTPSVwrRbdEBLHSXw5ae+m1R0LnN3do70Sv26qrPr3PikgcPEx1wJX91BkPREqgeMb4pnwDfJ1pykCV/dgsrZbDghcMlVvv2hMEKUoTChXlKgw6vtWB1RZ9e99UkDg4mOuBS6KuOmmES5n1hNhWODmSd6YpghcnVHl5wMLf15ZAgKXTNXbLxoTRCkKE8oVJSpMnY4Xqqz69z5NHFbgOJcb53qb6f7Afoece7vyWbJ5A4HT4A876gNk4qabRricswrcPMKf7bxTlzYahyq/3i7TxM2bNyFwCVS9/aIxQZSiMKFcUaLC1Ol4wWUNi0raYFHhz+AwAte5bJMrBC545mtPPtOV88Ltdj2yuo+pa18m9/JlOc1p2TIfnOu0aFdaW59cIXB++zIN+Pmy9x7TvXawvL8b5Ijj+bMQfpSWgvcrDT/60Y+a/SQGmQdu35TgF6cy05tM1bCfnhukZCiacDkhcBMgcAEqhU24JfKvUMPT1APsOW+h/CV1gWlvwqjy6+0yTUDgkql6+0VjgihFYUK5okSFSXO8MAUua1UCp+j3+2R7/kjMqgcPs9fIInDCw2WqBZk6pMdC1woEbpRTi1MxyPOeGGdxkzngOMVIj9ONiGnrJ2hdrn9Q9g5DuJwQuAkQuACWNSlooTxwrrUh2uhQtt0gQfXolf8Q4WHRTss4ZKny6+0yTUDgkql6+0VjgihFYUK5okSFSXO8MAUua9UCZxoQOI0sAtdSCXxlgtPgVSY+HYVMisrTVR44mf9tkiuO88IFiYCDXo68CJcTAjcBAqfokbXBbZDzwAUtj6WO870FbbMn27Zslyq5r2ioZbiRKr/eLtMEBC6ZqrdfNCaIUhQmlCtKVBj9eMGPgLLbHrVtl7p7Pg0GYrhtjZ/7yfdzdff4WaBtcrtdcs/9Fd1zxTnDvUft8xZZo/U2b3xCN6xz5HT3aNDrBK/iq9m2LdqbsfeKywqB2w8ETiOLwKWFL0Ox6JVFuJwQuAkQOPNR5dfbZZqAwCVT9faLxgRRisKEckWJCqMfL2zxh5vj71HH3iSLb9oXf8CdE3/cBc/9FNLWbguR65LD94C1feq2bPI7HfI4Ebjr0A2nJedzjjhelpdxHI86YhnOJTcYsBTO9h3lshYhcPrydQk+1kHgNIoQuLIJlxMCNwECZz6q/Hq7TBMQuGSq3n7RmCBKUZhQrrQCp9P2y831lgSXtQiB05lWJyYRJ3BPPPHEgUwS6lnZKiBwhhIuJwRuAgTOfFT59XaZJiBwyVS9/aIxQZSiMKFcUaLC1Ol4wWWFwO0nTuD4VZe4zz//fF+9QOAMJVxOCNwECJz5qPLr7TJNQOCSqXr7RWOCKEVhQrmiRIWp0/GCy1qVwKXJA8fpRWzXIcf39VmFkSRwHHHyxgGBM5RwOXl/sgQErt7UpY3GocqvH2zSBAQumaq3XzQmiFIUJpQrSlSYOh0vuKxVCVxcHjjO+SbvCXT5Pr+eFLxh/zLt+h3q2OdoVxyO+pwTbl9euPyYJnCqF06vEw4InKGEy6mELGvMIxA481Hl1w82aQICl0zV2y8aE0QpChPKFSUqTJ2OF1zWqgTOcy0hakLgWi2yrK4UNylwjk2+kDJO3MvTuQeuZblini3mtcYC53hC4Nx7uadimiZwSdF4gVPpUDmXG+fU4vEWpxJZP7F/CZmKgfPCuePliiRcTv1DmxZ8EtT3c16AwAWo3G5DzgMnU9hwCpxRG+YjzCjvW5C3UBy4xETOF9cbclv3R8P6u+aDKr/eLtMEBC6ZqrdfNCaIUhQmlCtKVJg0xwtTUGXl24LSBtc9H/cP+ygtU4HAaWQROJnvjfO48Qif9PgcyINiesAox5Z8OgMLHNE659QqOKVIuJz6hzYtIHD1Js1np3K7uS0rSN47dGUOOJkLbpTXUOWAox7/5TkUy66L9i1ehSBxDsOi2rAqv94u0wQELpmqt180JohSFCaUK0pUGP14YVuOzPOWhX6nLXueBgV/92Y5NkDg4qPxAmcq4XLqH9q0gMDVm7p/dqr8ertMExC4ZKreftGYIEpRmFCuKFFh9OPFrHng+N6vtjdZT+WB42VnTdyrM8uxoUkCp9/PPi0gcIYSLqfeoKcFBK7e1P2zU+XX22WagMAlU/X2i8YEUYrChHJFiQqjHy/kTfjUl/dvWULkztsO3WiFBK7TDu75GiXpZYHrekEPHD+BQa1nd/fI8YJl8/pGznJsaIrAvfTSS+P713lf9Xva42JuBa7uodCnTwsIXL3RP886xqycP38eApdA1dsvGhNEKQoTyhUlKsxhvm9lo8oaPlYcOXLkQLLa8K8upwmcPq3OqH1OewlVLXsYjBW4KOKmm0a4nPoHNi0gcPWm7p+dKr9+M3Ka4L8oIXDxVL39ojFBlKIwoVwQuGiB0y8rNjEOAwSuAMLlDDf2NAGBqzd1/+xU+VnI9LY5LSBwyVS9/aIxQZSiMKFc8ypwHLrEffnll+N5SQI3rxy2Vy0LtRY4+Uu+fT/G68lfnPJ0Oca/4quAcDn1xj4tIHD1Js1n13OtcRvdP33SXuUyFbRfVX4IXP5Uvf2iMUGUojChXPMscBxK4vTpELhiqbXAKXdT6USCk+IopxYPafnfeLgMwuXUG/S0gMDVmzSfncwDJ15V7reg/Q6D9jpKIWJZQa43zgHH+eI4B5y78cxoWH/H/FDlh8DlT9XbLxoTRCkKE8o17wLHAYELgMDFNGp9Op/sejL3G8sZi9t+gduX/03MU9OLJlxOvUFPCwhcvUnz2aneNZkHTgrbqP1alpCzocz95stcb4G4tayWzAHXk8tbhSaiVuXXBW5paenAfS58UgovA4FLpurtF40JohSFCeVKK3Bt+cxPM0VHlVU/ZyUFBK5Yai1wphIup96gpwUErt7M8tlJaQsjJK0qDVLl1wWOQ5c4fT4ELpmqt180JohSFCaUK63A+UJyOI3I5o1PaNf3ZGoQ7/xfyWd49jsd8jo9ant+brndsqDKqn/vkwICVywQuAIIl1Nv0NMCAldv6v7ZqfJHCRxHXO8bBwQumaq3XzQmiFIUJpQrrcAFz/zkPG5datm2zOnWtdu0J75W/XabNi2HPMsVopftaQ15oMqqf++TAgJXLBC4AgiXU2/Q0wICV2/q/tmp8scJHPfCRckbBwQumaq3XzQmiFIUJpQrrcCZjCqr/r1PCghcsUDgCiBcTr1BTwsIXL2p+2enyh8ncEkBgUum6u0XjQmiFIUJ5ZoXgbtw4cKBpwmkCQhcMUDgCiBcTv0kNy0gcPWm7p+dKj8ELn+q3n7RmCBKUZhQLj72NT0gcPkDgSuAw5RT/cUyj/BnO+8c5rM3AVV+CFz+VL39ojFBlKIwtVxgPoHAxZwE9ekqLUiQ0LdH62Kcc73JHFs8jfOHcDqGUY44mXaL/8kUDT0ZRRAuZ9YTIQSu3uhtNAqVLoTzuq27PlncZoX38Lgvc8QF+eGsgtpnEqr8LBv6X9DTAgKXTNXbLxpTRcnUcoH5BAIXcxLUp4fzusl8WvzaWpfCFghcazyulpF54WR+ONKe4pAfELhoIHAKzvnWCvK6scjJNjqknhC7QO4myX3LRpVfv4clbUDg4ql6+0VjqiiZWi4wn0DgYk6CcdNNAwIXTRMETu+ValpA4OKpevtFY6oomVouMJ9A4GJELW66aUDgouHPFoCqqFqgqt5+0ZgqSqaWC8wnELgIsalbKHSB0x9HpGe0h8ABUAxVC1TV2y8aU0XJ1HKB+QQCFxKgMHHTTQMCFw0EDlRJ1QJV9faLxlRRMrVcYD6BwMWIWtx000gSOF3i9HkQOACKoWqBqnr7RWOqKJlaLjCfQOBiRC1uumlMEzglcfo0CBwAxVG1QFW9/aIxVZRMLReYTyBwMaK2fzrnfAvyuUWhUoVEzy0WCFw0EDhQJVULVNXbLxpTRcnUcoH5BAKXSuCm54HjhL3rrVFOLTGzJ17lslLuVD44V07ndfMijcDFBQQOgGKoWqCq3n7RmCpKppYLzCcQuIwCx/LWEtbW6vVk8t5A4NalvAUSNxE4mcx3nOR3InAq2W8eQOCigcCBKqlaoKreftGYKkqmlgvMJxC4lAJnKhC4aCBwoEqqFqiqt180poqSqeUC8wkELkbU4qabRricvD9ZAgIHQDFULVBVb79oTBUlU8sF5pPGC9w8ofewpQ3+kQMTftVlD5EuAGCqFqiqt180poqSqeUC80mjBQ7ML1XIVBXbBGZStUBVvf2iMVWUTC0XmE8gcGAuqUKmqtgmMJOqBarq7ReNqaJkarnAfAKBA3NJFTJVxTaBmVQtUFVvv2hMFSVTywXmEwgcmEuqkKkqtgnMpGqBqnr7RWOqKJlaLjCfQODAXFKFTFWxTWAmVQtU1dsvGlNFydRygfkEAgfmkipkqoptAjOpWqCq3n7RmCpKppYLzCcQODCXVCFTVWwTmEnVAlX19ovmZz/7Ge3u7uqTKwcCB8oEAgfmkipkqoptAjNJI1DD4VCflBtptl9nVM5K04DAgXnFzG8cmEuqkKkqtgnMJI1AQeBmBz1wAJQLBA6URhUyVcU2gZmkEagyBO7s2bNzGUtLSwemxcXe3p5WO8WwurpKjx8/lgHAvAGBA6VRhUxVsU1gJqYI3M7OzoFH581D8P7p06JiY2MDAgdADkDgQGlUIVNVbBOYCQSu2IDAAVAuEDhQGlXIVBXbBGZimsA9vbxEiwsLtPMwEJvlpUV65U879FATnndfWKbtnYcHRGgS79IL4r34RwQLp96mB4nLjuLu63TslT/J910R6z4lhg8skzEgcACUCwQOlEYVMlXFNoGZmChwx556aiRt79Lx8fAdWlpckEL26tPLwfCpd4RsBdNPvbMtpY+Fj5d5Z/sqPS/ea3uHp52itx/s0OvfC9Y79fYD2vn4dSmKCwtPifXujt+bpY0F7ntiXV5uZfk4Lcn3DJbj95BSKGL76gtS9BZOvn1AyEwSuO3t7QPba1KY+CMSUBwQOFAaVchUFdsEZmKkwL3yCh0XIrVz7Xl659XjUuA+fu1pIWU7dO2FZfrT9jXZu8aidee178npS8dfEdMfSoELes4mPXB/2g7eW4ncslyWpwVC9qdXnxbbEAJ4Z9IDNxE4sZ2PX5PjH116WrzHMdp5NxC3V48LITz2yoHewXBA4KoPCFyzgMCB0qhCpqrYJjATMwXuT6NLqU9JcWOBu/b8shS1QJYCOWPR4kupPF2drFnguDdusoyYtsi9eNviNRC4sbhtX6UXVkICJ8Qsqgdu5+G7cjld4NQl3OWlk7ESZ4zAvXuGTl65T9ujS9NpY3Vlaerl57WV0aVq7pXcOTh/5fQV2gp9RjKun6WnLt6i+0K6n1ldFts4uF5eAYFrFhA4UBpVyFQV2wRmYprA1SKkwLHYRczTwjiBuyZeTx6Tl4Uv3v4TXfreCt0WUrty/KJ8vX5mlVaWhIw9dZFuifETq3yv4Vd09/UTgaSdvEKvn1gVMrwg3m9LzmOBC8TtOh0XUvZg546cLy93f291vB6/F1+2vnjrAW3feYNOifVZ3KTAXTsbiKBYbktK9sE6mjWKFrhz584dSAnTtDAJCBwojSpkqoptAjOBwGWIu6/LnjkWkpNvPzg4PyJMFLinLt6mBx+/RsfE658ufY+u3H9HiNdt2dO2unxMCtaZ1RUpY1LgxDrcE6d611aXT4l1tqX0seRNBO4uLZ+6QrdeFe+5tU3XzqyJ+VdpiaVs3AMXLLP1p9f2CRyL3vKxi7E9mYeJMgRO32aTgvffJCBwoDSqkKkqtgnMxBSBe/jw4XganxS++eab8XAdCJfz008/HQfvX3g8Kvj7+Itf/IIGg0HoHfNDF7iLtx/Iy8KBtN2hp489JXvfWJ6iBO6j106I6YtjgVtZOikFTp3AlcBdF8LG614VryxwO3L+9ZHAfSwFbSx0msCpS6jyvbfSyXHagMAVGxA40FiqkKkqtgnMxESB+/rrr+mzz76Qw3yCqAPhcrKUqZNbmh64agWOU6Ysju9zG19CFZJ1f3tyCZWn65dQ5eXO7aAHLrgH7uRI2u6OL6HyfPV+LIH8a97FuB44sY66L06vo8NEWQJ3/eyRUT08mf5ew+tn6cmXbx6cruLuG7Q0ur/w5Q+39r/v9Rfp6NqyqON86ytrQOBAY6lCpqrYJjATEwXu22+J3n//fxHb3ZMniDoQLqeRAhex3aZEGQJ3941npMhO5PMuvfHsmhQvvr/vyCqnn3mSnnxyiW5ubdO7Qvb49aEQtFNvfSnWfzYQ4efeoi8ffBxIL6/7cTD/wc51WhbzvnjnhZEkPkc7Yt1nj6zQfSHBR0YS/cX9j8V2A5F87tdfSMFbeO7XYnpxtyhA4EBjqUKmqtgmMBN+VmeSOCwvLxd2bxYTJ3A///m/gsDlBASueIHj7Yx74IRoBUJ2iq6xqL39PK2tBj1lPP2tL9+hF46sBj1nI4FjwXtOvPL7BMs8oGsvHqWbty6Ne+A+ZOG7/iI9+fKH43WlwF17Ucoaixyve4S3K9ZdO/4cLR97uZD7CsMBgQONpQqZqmKbwDwGgwUhcMHJIQ6et7e3IEQufpnDEC1wj+iDD/4vIXADeYII0/M2aTD0yN60yLJc8gc+bThdsuXwkPqeTZbrU2/ok+f15PRh3wu9WuS7HnmuRa4/oGHPI8f3xXoe9XyXWo4v3j97j2O4nKYJHP9AhOFt/fnPf9bmmo+q288+++zA/YPT4vz584X+AcLsuwdOSNUzayv0zgtHaHX5ueCS8vWgBy64z+8uPX3syUn+wJHAce+bEjgWQRY4ue5o/jjNihC4594KZE0J3O3Xnh0L3HUWN7XdUfD4r7+4f6Dd5RUQONBYqpCpKrYJzCPofVtIFAfugWN5K+okGCVw3333nRC4/xwjcB5ZtpAsR4iXEDVfuJYlRMyVw0PybFuI3CAQONuT0y3bIsdpkWPZ5PJrS8hfqyXfRw5bgcBZVjA8GOQvcCsrK8ElsQcHb9AvS+AePRrST37ykvgsi+2RyhtVtyxwet1Ni9IFroEBgQONpQqZqmKbwDxMFrg//vFUpMDlTb/fJ8vz9cmZCZdTFzglbyr0E2BZAseXpm/e/HdiO/V6BqqqWwicmQGBA42lCpmqYpt158UXXzyQvHIeYnFx8cA0Pc6cOSNfixCMKIHjk4Lr3pA/nuDhOhAupy5wYXmL6oUrT+D26B//8WVRr9E9cP1Oh1yrRW63K8riUafjktMdyEvLth1El3s3RQw7Nu31OtQW04bUp80bn9Cu71F3T8wX7+M4DvnyknRX30xmVN2aKnD/8A//cGC7dQr+nh3mh0o///nP9UmVAoEDpVGFTFWxzbrDAqcf+JoWRQhGWOD07dUtFNMETpe4sgSOezZ/+9ubsmczCha47sARouaTLwTsnOsKieuQJYa5bHz52XK7gcD1bXKstpy2NxiSJyRp0BfLdnfJsdtS4Cy5fk/fTGZU3cYJ3NraGm1tbR2YzlGWwCn4HsNvuauT6pMCh1ECp99DOC3efPNNCBxoLlXIVBXbrDsQuOIFLgrebh0Il1MXOL0e9ShX4N6Xv+6tE6pu4wROSXGUxJUtcP/tv/2I/vqvT8hhVe46oAROr79pwfsOgQONpQqZqmKbdScscEucpPTU1QMHs2mxePoq7Ryip2np9DuHWv+wUYRgQODKFTjX/f1cCRz3viXdX1i2wN28+e/p2rV/KYdVuesABA6AGahCpqrYZt0ZC9y7z9OrV1+l44uLQjqC8WPH+ORxiq7uPKTjS5xtfoFe/eihTBOwtMjjx0bLBOK3tHiMTp06Jtc5dYrnnxLL3qHXng6ScZ66uiPfd2npGC0u8nt9TJeOj7Ldn3pHPoich8uWuSIEYxaB6x3ifh2G1+f7tg5z349OuJwmCpy+zboFowucLm9RvXBlC9zvf39RSNy/lcOq3ApflINbHN83yPcK9jttsl2+PO1Rtxu6z5DvIxz4ZLc9Mb+7/x7EXV6vM0p506XdGX4xHQUEDoAZqEKmqthm3VEC9+7zS/TRw4/pNSFUUqCEaLFwPS+k6tirH40ObHfGvW0scFL0vnqXFk9NpvGy4dePLz0tXk/TNfH+i8deHQncIu3wQ8fFMjsPr4n1uQfujpz/0ejRR2VGEYKRXeA8sroD8j17nMeN879x7rfBsCdzwnVdThviymmc/60nlvX45vzNZ+V8PvlxypBZ8r3FES6niQJXZ1Td6gKny1uUxJUtcL/73U16//3/Rw6rcis6bdFmu3vjewk77baUOr7f8Mci9kL3Gdpuixx/T87nexDD81jg7NH9hbOkvIkiSeCOHj1KX3zxxYHpHBA40GiqkKkqtll3pMCxVCkhuxOI1cNrgcBxz9uxV68GciVlK0ngjtGrH+3Iebzu02Ldqyxui9wTNzo4jnrgHor19gtcIG4sku/sBL18+kG1qChCMLIJXE8m72Uh29zYkMMtzuMmRI5zv/EpiIc535vnDckXr5wrLsgN55A/WpbljfPC5XXyY1Q5f/jDH8q0CipYzMLjcQGBi0fVrS5waaJ8gfuAPvjgP8jh/W3XbJIETolxlMRB4ECjqUKmqthm3Yn9EQNfUv2o/N6wKqIIwcgmcOYSV07+lV4aTBA4zokXpbTyV6fh8dEl6DwFOAlVt3UQuM8//5I6nf9XDse1CROJEzjufQv3buoSB4EDjaYKmapim3UHAgeBSyJcznCaBd4/PfWCHqZcQk2dB06OWzJ9SBmouq2DwH3zzTe0vR205bq0XSZO4PTL0xzh+RA40GiqkKkqtll3lGgwX3/9tTxQM3U6SCt0gZgWvO98Iszzpn9FE/LA6cvpYZLAqTxw5+wu3WjZ8tFi/Ngxx+mS43bl/Yb8PFm+BL23l397iELVrX7ZOW2UKXBhwm3CdOIEblpA4ECjqUKmqthm3QkL3Bdf3KfPPvtCDtfpIK0IC0aaKEvgoqhL/YbLWVeBMxVVt7NI/q1btyBwKYDAATADVchUFdusO2GB+/DDf03vv/8v5HCdDtIKCFz+hMsJgcuXw7SBCxcuQOBSAIEDYAaqkKkqtll3wgL3wQf/Rhy06pesUwGBy59wOSFw+aLqlu+B0y/vT4uy74ELo7fdtm2R040uy/4fivTlZeu458jyj034hyRlpRFJCggcaDRVyFQV26w7YYH73e/eERL3H+WwfpD2NsSB1x9Sz7PIFQdhmadsOBrfeIY2HL6nyCXXdcU8zkfGaTHEPN+X8/k42vM8MV28j+vve++8qLPAeZYVDIzyv3FKEZkqpD8kX9abKx+izvOCZL09EZ686T6cNy6oY0ucJPOp43A5IXD5ouq2Dj9iCKPKrciSB07JWU8cJz4Ry3j8A5MS8sBxW8waEDjQWKqQqSq2WXfCAvfee/9DCNx/lsP7D9IsC0rWNsQB16EW/yXdG5IrZKFncR4yMX8oBM4b0gYn6xTywfMsqyfzlPFhlOXCFfNarXzkQidO4JaWliKz9pskcFK83IGoN1uIMckcb9Sz5YmMhx0WZNeS0qyetrBpOeTIvHG+mB/M4/fhG/F5Wh6EywmByxdVt3UXuFloj5L55v/N2w9/T27fvn2g/tJE+NhoAhA4UBpVyFQV26w7+wXuA/rjH0/J4biDNPcUHfaRT0URJXAsbypNgC5xJgncLNij5L75l35CuJwQuHxRddtEgSuLw3y30QMHGksVMlXFNutOWOC+/vobcpzfyOE6HaQVUQKXlOupKIELb297e1ufLalL/YbLWVeB69gjye1zOpEB9TrB5WmZ/60bBA9zzyZfvgsu4Xnke215aXDQ6wTP8eRLfDZfno6+hysrqm5NFjh9u3UK/l7jHjgAZqAKmapim3UnzYnY5AijC1y49y2qFy5PgeP34vbH22SWl5flKwucXua6hSIPgePP4PHjx+Pxw5BW4Pjmebc7IFeIHN9rJR9JJobPWZa8R4sFj+/DGgwCeZMPVXddctvt4B4tvhXA5WfRerQ38OUyeaDq1hSBO3LkiPx8/vmf/1mOz1MPnF5/0wICBxpNFTJVxTbrTtx9HnU6SCt0gZsWeQtc1HiRl1DLIFzOWQUukFk+/bC8LdDq6oKclzWUFK+ursrX+/fvj8uWK522vEeLb8Y/fMuIR9XtrAK3srIiZYvFS73+9Kc/PVBvaeOf/3lBvEfwOXEEwwdR5a4D+BEDADPAX4CyqWKbdUcXD0WdDtIKCFz+hMs5q8BxD5zqAeXT0Orq7Kci1YOnelPrjKrbWQWOe+DC8hbuPZsFFjj+fNRn9Xd/93f6IhJV7jrA3+2XXnrpwFMs0gQEDjSWKmSqim3WHV08FPpBmn/deNgfL/B7cLoLr3fwfcZpNA5BnQWOf+HLaUCi6HO96RMFluWQzz9ZPUCfPK936M+LCZfzMAJXBBC4fC+h6qS9hCpzvXU65Mjnze5Rv9Mme/P79Inrkd32qOtY8vIzP2t2cr9hOeBXqADMQBUyVcU2607cQUo/SOeTB86ioc85nzjliBAM15LrtNyeeM8Nsexw3/tm9Y96C5xHli32XZzsBr448Yn6Ywmb5IGzyPEHot58Od32/Mkw54gb8LM81WuQB85zRznixO7xPMfPnlokXE4TBU7fZt2CmReB+2TPEcLmy0vPfIzwhcg5/h65dps8Mc3i+w15Hj9vNuaPlbyBwAEwA1XIVBXbrDtxB6n9B+n88sD1rGfF/IHMB+e0OCWJED3xurG+Lt5TiJzL00iumxX9EkjaMEXgGL653vNsUQ++lgfOkbndPHHy27Ac8ljWWOBszq3ni+FekP/NHyVNHT2s3bJ9+R5qWlbC5TRR4OqMqts333zzwP1X08IkgTMZCBwAM1CFTFWxzbrBv5IcDIJ7XPjEGneQqtNB+jD84Ac/MELgTCVcTghcvqi6xcPsiwMCB8AMVCFT+l+piIPB8ra0FNyszK98go2iTgdpBQtGljDpEqqphMs5S0+REQLX75DTHZB+1U6mCwmVrd+xybac8f1ZPD+8Cj+rU+WKywNVtxC44oDAATADfPAG5qFyo6mTapqeFJMjjKn3wOnbrVswLAzhfUnTbngdXq5ygUuZB67rCoFzW/KeLX7ME9+vxc/x5Nci88DN0kYgcOmAwAEwAxC4ehB3kKrTQVphqsBFUZf6VeWss8BxT9r4ObOjewqHfZtutOzgPkGXn8zQlU9a4B459TB2fnWcrpzP6/H84Dmz+T6JAQJXHIf5biONCGgsELh6oIuHok4HacVhBI6TxKr8V8vLC5lPjno9QuDMEThTUXULgSsOJXD67RPTgm8XgMCBxgKBqwe6eCjqdJBWzCpwfFl5by+4L5ADAjdBlRMClz+qbusucLbVIqe7R23Pl+lB1GuYg/cbtuU6+n2JeaMETq+/aYFHaYFGA4GrB7p4KPSDtEoKy8k4LSvIA8d52ziX2yGuUuTKrAJXRA+cIq3AqUTGnE6Ec7v5ol45Bly5fS+4hMfpRTiv3uaz8n4sXm7oB/OGo2X4lXPA+WK+y58TfzZiGueAm+VykionBC5/VN3WXuBch7riM/ZEefiT5ld+jizfN8iffZ9zv/F4q0XnRTu/MbrfsCWGu7wOX7YWgte5fJn8XYd86zJ5u7v7tjErEDgAZgACVw/ixEM/SCuB4/xvnKzXbVlB3rZW9nxtRXEYgTsscfWYVuCEwcnceb44qW1YvsyRJ+/V4llC3Di5MY9zXjfOq8fJfWUOOMeSOeAsm3PxDeQN+pzvjdf3B844jxxP4+GsqHJC4PJH1W3dBc5kIHAAzAAErh7EiUedDtKKWgtcgXD6C8vzg968jKhyQuDyR9UtBK44ogTu6NGj4972cISXgcCBRgOBqwdx4lGng7TiMAKnLqHyvXBVXEI1FVXOOgscpwkZUofsjk+9gU9epyefx9nr2OTx8zvP/RVt3viE7M1naePGb0R5Per0iylzGFW3ELjiiBI4Dl3ivvjii33zIXCg0UDg6kGaE7HJEWZWgdN/xMAHdD5460lpp0UUELiqBa4vZI0fUxYk4OXLyrbtC6nbJMfm9CAOdVtC5EQZ+fmdrnjtua4cL5rDtIELFy6UInD6Z1qnYHlLI3C6vHFA4ECjiTuhAbOI6znig1jdmFXgwpdQuScuzxMjBK5qgVMIgeunS8Db9vdoL/vV5syouq37w+xNJk7gOJTE6dM5IHCg0UDg6gEELp9LqHFA4EwROPNQdQuBK45pAhfV+8YBgQONBgJXD9IKnNVy5S8hFZzuIhjvyUtO6leqk/n868jhgemcLkOflhcQuPwJl1NPdjot+Bjwi18Y8CxUQ1F1W3eBmyUPXKdtU8vpHlgub5IELikgcKDRQODqQWqBc1vUcv2RgI3SiAhxGw6DVCKcXsSyejLFCOcn4/xwvHywTHAvisepL6xnaaPl73vvvKhS4OLqMa3AqfxvKq+bmCBfbVHfnNetz/NFnbr+QObg47xuLMJDz6aBGLctUe98E744KfKwfI8cCJcza/1C4JJRdasL3Nra2oFfSHJsbW2NlzFK4GbIA7c36Mm0NjL/m4ii2ggEDoAZgMDVgzjx0A/SSuB64gDsimHOMxbkhGvtFzg5zIlkRwI3yhvHx1GWv5aYzzeL56MX+8kqGHneAxdXj6kFztqkH3dHedxEvQ2EyHFet+HQl2Inc7lxHds+OU4ge1Lg+rZc1rM2yOeHtPPN+Py8TjcfSQ6XM2v9QuCSUXWbRuDC8sZhnMD5msB12vKPCFf8QeGKdunYYrzbEvJmi+NBV3z/+4HAsdhZPu3uFnFEgMABMBMQuHoQJx76QboOZBWMKIGblbh6TCtwWVB53cogXM6s9QuBS0bVrS5wHLrE6fNNEjiT4e/2Sy+9ROfOncscEDjQWCBw9SBOPOp0kFbwvuipPaZFHQWuTMLlDAvcysrKgV4ijgcPHoyXMUXgWHiHIvqjz5nvx+Ih2wou/YWnlYmq2yiB44jrfeOoUuDqiF5/acK0/YfAgdKAwNWDNL8mNDnC6PPSxM2bNwsXOH2bdQuF3gOXJG8cxggcy5mIc5Ylc8A5LVvme7P5VgB/QD7fv2Xbspyc9Jfv3Sr65npG1W2cwHEvXJS8cZQhcMAsIHCgNCBw9SBOPNTJpU7oJ7k0UYbARVGX+g2XUxc4vRdOr1tTBE5YGbUsV95/qQTO6g7kryL5k1cC5zhdIW+c3HdYSm+cqts4gUuKJggcp/n4y1/+ok9uLBA4UBoQuHoQJx7q5FIn9JNcmoDAJRMupy5wHEri9N43DmMEzlBU3ULgooHA7QcCB0oDAlcP4sRDnVzqhH6SSxMQuGTC5YwSOI4oeeOAwCWj6hYCFw0Ebj8QOFAaELh6ECce6uSi8DZa1PKHMkEvp6iwLFfmKpOvhqCf5NKEKQLHKVYkMsfbgHzPljnfbM+nwZATIgfpRCzLIcdxZZoW1/fJ3XyWuq5HPd/dt14OuyQJlzNO4OKiDIHTt1m3YCBw0UDg9gOBA6UBgasHceKhTi4BPSk5LG/u6EkKrpS4ICdcTq5waPSTXJowReCEwZEj6pTrl/NicZ49y3bF5CApr+e0aENM54eve96QNjc25HK+WE7mjpN5+NR6vsyxlQfhcpoocHVG1a2eviJtzLvAMU3Yx7RA4EBpQODqQZx4HBCMGqALRJowRuAywjfdH77U0wmX0zSBK+p9y0avt7TRBLlpwj6mBQIHSgMCVw/ixINPEHVDP8GliboKXFmEy/nmm2/K73WWKFLgqobTfDx69EifDHIEAjcBAgdKAwJXD+LEoy6CEUaXszQBgUtGlfOHP/zhgUt4aWKeBY7ljSUOFAcEbgIEDpQGBK4exIlHXQQjjC5naaJogdN7pJoY8ypw6IErHgjcBAgcKA0+cAPziRMPCFw24uqxalSeNpA/6IErHgjcBHyLQWlA4OpBnHjUUeD4Jvsswfte9LNQqwYCVxzogSseCNwEfItBaUDg6kGceOgCF84D57ZaNOx55KvUIhvP0Ibjk9VyyXVdMc+V+eHkPM5VJuYHjsTpSHqjfGYtMW84fp88yPorSQgcOAz8o455vTxsChC4CfgWg9KAwJnL0tLSeDhOPHSB2xDStt7ivGRBHjiWNBY6mReO88HJZLNC4LyhXNZxhaBxTjKrJ3OVBYrEAse5aj25jCteWQZbrp9L4lkI3EEgcMUBgSseCNwEfItBaeg3MiPMiMFgQQgcHwqCV/6VYBS6wCmUwB2KUULgvIHARWNy2eoMBK54IHATIHAANBwlcBzcM5NV4ExGFzjuaeR9DEf48UsQOHAYIHDFA4GbAIEDAOyDT+56z1SdIgwEDhSFLmosb6B4IHATIHAAgJnugasDusBx6AIXngeBA2mBwFUDBG4CBA6AhtOkS6h6L1y49w0CB7IwTeD0cQDyxgiB02+qRiAQ5cVhf8RgMlECpyROlzcIHMgCBA5UjRECBwCollkuoapfnnoq55vljvO/9YY9cjm3mxukCamKOIGLCwgcSAsEDlQNBA4AsI848dAFznU5X9tQ5nyzODmvGPfl8FAOb3ASXxGHTjFyCCBwoCggcKBqIHAAgH3EiYcucHUAAgeKAgIHqgYCBwDYR5x41FHgeF/0e/6mBQQOpAECB6oGAgcA2EeceNRR4M6dOzdTQODANCBwoGogcACAfczSazVvAYED04DAgaqBwAEAQAFA4OYbCByoGggcAAAUAARuvoHAgaqBwAEAQAFA4OaXtbU1+SSPR48ejaeFx6PmA5A3EDgAACgACNx8wnL26FHw5JK1teAUytPUOEtbeD4kDhQFBA4AAAoAAjefKIHT5YynhXvgopYBIE8gcAAAUAAQuPmFBU2XsyiBg7yBIoHAAQBAAUDgyufs2bOlBD87+IUXXjgwTR/Xlyk6dnd39SoBcwwEDgAACgACVz660DQtIHDNAgIHAAAFAIErn+3t7QPPt21SQOCaBQQOAAAKAAJXPhA4CFyTgMABAEABQODKhwXu7usn6OLtB7Tz8KDgHD7u0vKpK7S1vRMxr/qAwDULCBwAABQABK58VA/cytKCTKR7f/uhFLor97fp2plVuv1gh1ZXluipi7fl9NXlU/TC6godv3iLTqwu0/a1M3L+9k4gRDyf1105fpFuPWBpmwhcsP4xunRilZ5+5RatyfWu05m1Fbp9KXjva2fWxLonaemYWP9+8b2DELhmAYEDAIACgMCVT/gSKgvWySv36Z0XVqXMcbCMsaDx9K/uvi6k7Wl6euVpeuXWAylwH70WiNfO6D2UCC4snKQrW/ze4R64QNaOLwfrr60cp+2Hd+kNIXRXTq/sW3dFbJOHt6QEHhSvvAIC1ywgcAAAUAAQuPJRl1CVsHEPnJSuxcn4WOC+CiTvqYu36MHOw6AHbucrun5mJHwnr9DrQsbkuieVtGmXUK+fGa/PPXByvacu0sOvApEL3ufivu3r0pVnQOCaBQQOAAAKAAJXPqX/iEEIHPfMcY9d0AMXsUyJAYFrFhA4AAAoAAhc+ezsFHuJsoyYlQsXLtDe3p4+GcwxEDgAACgACFz5sMAxLEJff/21Ntd8lMB99tln9Omnn2aK8+fPQ+AaBgQOAAAKAAJXPkrgHj8e0Ouv/ychNPW6pBgWOL1nblpA4JoHBA4AAAoAAlc+SuC+/ZboD3/49zQYPNaWGNHvkNMd0GCoT+5QbzAYj3dsm6xWi/b0BUf0+30aUp86nZ4+ayYgcCALEDgAACgACFz5hAXuv//38zQcRvfAsZhJJRMi1xXC1uvY5Lg+2bZL3W4QPKzkjGMw8Mj32uR292jQE+vt8XodsZxFLaerb2ImIHAgCxA4AAAoAAhc+SiB++677+i3v/1QCFy00HBPm+UOhLTZ5A+G5LHQ9W260Qp63CyXha476V3rt+m87VBHSJJl+eQ4wXoscI4jlrfKEbi1tTXa2to6MB0C10wgcAAAUAAQuPLZL3DvxwpcZjptavt74qUd9NwVxDSBU/ntoiQOAtc8IHAAAFAAELjyCQuc4/x/+QlcSSQJHPe+KYHj0OdD4JoHBA4AAAoAAlc++9OIfEOD0A8S6kCcwOnyFtULB4FrHhA4AAAoAAhc+SiBqysQOJAFCBwAABQABK585lXg0gQErnlA4AAAoAAgcOUDgYPANQkIHAAAFAAErnzSClx+eeDatNu1qO3tUVssz+O25ZAl3mvP9+QyWX61CoEDWYDAAQBAAUDgyietwAlzEyI2IJeftOB2ybFsmQvunGXRjx1HCp7rtIS0BfImn9DguuS22+SJYVcsx+t1XU+IXotazp5YZ5Mcu03+nkO+1aZzMp9cN/YpDlEogXvzzTfpl7/8ZaaAwDUPCBwAABQABK580gtcRkrOA/fw4cMDPWzT4tatWxC4hgGBAwCAAoDAlU9hAlcSSuBm4cKFCxC4hgGBAwCAAoDAlQ8LnN4zVbdgcA8cSAMEDgAACgACVz7z0gMHgQNpgMABAEABQODKZ14EDj9iAGmAwAEAQAFA4MpnXgQOP2IAaYDAAQBAAUDgyietwPU575vv02AY/ZtSmTYk9BxVfbwoIHAgCxA4AAAoAAhc+aQXuA4N+zbZ3kDme+N8bZwXbiAkjXPAcU44q9WSOd98y5L533whRzyfc8Bxrre9vWj5OwwQOJAFCBwAABQABK580gucTS3LlU9hcIW8+YMeeWLccbpkjZL6WhsbYtinbsseJejdE7LnkCPGIXDABCBwAABQABC48kkrcKYCgQNZgMABAEABQODKBwIHgWsSEDgAACgACFz5QOAgcE0CAgcAAAUAgSufeRG4WcCjtJoHBA4AAAoAAlc+8yJw/CSGTz/9NFMgkW/zgMABAEABQODKJ63A9TvnaG/gkX3OIssOfo1qi9dhv0O251Nv4JPX6ZHtdmnge9TdGxD/5tS2HLJcn/ZG03qdNrndLrnn/oruuR4NOx1qu5aYtke+16aNG5/Q7iD9r1XDAqdfIp0WELjmAYEDAIACgMCVT3qB68j8b67r0I1RHriOvUmObct8cP7AEULni9dBkFJEzN8TIsbpQ3yrTedawTqOZZPdDlKN+JxbTsR5y5LTON2Iy7nj9I0nAIEDWYDAAQBAAUDgyietwJkKBA5kAQIHAAAFAIErn3kVuIWFhcjY2tqCwDUYCBwAABQABK585lXgONbW1vbJmz4fAtc8IHAAAFAAELjyYYHTxaZuwUDgQBogcAAAUAAQuPKZ5x44JXHhy6YQuGYDgQMAgAKAwJXPvAtcUkDgmgcEDgAACgACVz5pBa7f78u8bkxvEOR4k9M7HTleFRA4kAUIHAAAFAAErnxSC5wQNac7oIFn03qLc8G51OcccJzzrdWic5Yl88MNMiThzQMIHMgCBA4AAAoAAlc+6QVOyJqQtE0hbzx8zmqJcVsm8vW7LSFvYr7bHffMlQUEDmQBAgcAAAUAgSuftAJnKkrg3nzzTfrlL3+ZKSBwzQMCBwAABQCBK595Ebhz587NFBC4ZgGBAwCAAoDAlY/eK9W0gMA1CwgcAAAUAASueTx6tEBrazitgnJASwMAgAKAwDUPCBwoE7Q0AAAoAAhcs+CnJLC8scQ9evRInw1A7kDgAACgACBwzYIFLjilQuBAOUDgAACgACBwzYPTfwwqfJIDaBYQOAAAKAAIXPOAwIEygcABAEABQOCaBwQOlAkEDgAACgAC1zwgcKBMIHAAAFAAELjmAYEDZQKBAwCAAoDANQ8IHCgTCBwAABQABK55QOBAmUDgAACgACBwzQMCB8oEAgcAAAUAgWseEDhQJhA4AAAoAAhc84DAgTKBwAEAQAFA4JoHBA6UCQQOAAAKAALXPCBwoEwgcAAAUAAQuOYBgQNlAoEDAIACgMA1DwgcKBMIHAAAFAAErnmsra3Ro0eP9MkAFAIEDgAACgAC1zxY3ljiACgDCBwAABQABK55oAcOlAkEDgAACgAC1zzQAwfKBAIHAAAFAIErn7Nnz1YaS0tL9MILLxyYXlbs7u7qVQLmGAgcAAAUAASufLa3t+mrr75qbEDgmgUEDgAACgACVz4QOAhck4DAAQBAAUDgykcJ3JnVFVpaWKAr97fpYYToxIVc7+SVA9PTxpm1YP27b5yg1eVjtP3w4DJFBgSuWUDgAACgACBw5aMEbnXlOF288gqdunJfShSPP/XUIi0snAyE7u7rtCgEb2HhKdp594yYvySHFxd52gLdf+cFuc7SUrDO0hJPP0k7Yt3XT6zSslju5JUt2r52hk6ePCaXu3jrtpzO69969XtC4BbpwfbDkVzdpTeeWZXzHuyI8etn5bDc5rGLdEuI5srxi3RidVmUeUsI4DNy/QUhg1sPPh6/L6+7Jpbh4a0HOxC4hgOBAwCAAoDAlU8gcO/K3rdAkE7KXjgpdLcfCPlaofssVULagvlK1pZoeyfUA3edpe74WO62P35dyhUvsyJlbvTeV04LcXtAOw+v0/GLt8Y9cEqogp64RXrnhbXxele2xPJC4FgAWcieWX2aXhGy+cqt+2I4ELjV5VNyOX6P62fXxmXlaRA4oIDAAQBAAUDgykcKnJCuk6OetxMry3Ts4m0pYTyNxx/s3JEid+vBVXpBCFt2gRPitjW6104slyRwX919Q0pZIHBBD16w3kTguGfu6WNPyV64icAtyvlK4KT0hUSNp72lTYPANQ8IHAAAFAAErnzifsSgeuD06fMWELhmAYEDAIACgMCVDwQOAtckIHAAAFAAELjy2dk5eF9Y3WJWLly4QHt7e/pkMMdA4AAAoAAgcOXDAsf8+c9/pn/6px4Nh0NtCbNRAvfZZ58dELtpcf78eQhcw4DAAQBAAUDgykcJ3KNHQ/rJT14SQlOvS4oQOJAFCBwAABQABK58lMB9+y3R++//GxoOH2tLTOgNBnSY/rm2bZHTPShM/U5n3/v2+30x3qdOpxeaGg0EDmQBAgcAAAUAgSuficDt0T/+48tC4OJ64Dpk3RtQ17PJ7Q5o0OtQVwid7Tr0/7d3N7lt42AAhrNpTjXT40ztAj3LiNQFglxhTIrodjCLnqCVrF0XRbMJsumiEP0NKdX9UdNGci0hJd8HIGxLcpLlCwb6ZDZ/yhurRSsjyjbim0rqboi9ShVit09ls3stTYileKzUtj8fv+t9JWVpRcdj9XB8H4JOh9grTD3+I75DwGEOAg4AFkDAre8YcB8+fJCXL/8LAXdf0LQhqCppnZbt5pkoZaUolKjGSxMibq+1mLAab6RR4RpThPeHPtaazkhdhLirO6ls+I7txNq6Px+D7rmuZRfOmxBrysafcegDzoSfodSvB9zl5aXc3Azz4caLgMsPAQcACyDg1vdtwP37g4D7woVQ+5V/o57bQwF3fCLDfRFHwOWHgAOABRBw6zsG3N3dnbx7916896MrHrefBVzcfTsGXFzj8wRcfgg4AFgAAbe+VObAjQNuHG/37cIRcPkh4ABgAQTc+o47cL+rHwXclEXA5YeAA4AFEHDrI+AIuJwQcACwAAJufQQcAZcTAg4AFkDArW96wP36HLh9E+e9Ken8+e5jJeAwBwEHAAsg4NY3OeCclr8KI7tCSeVciDQrqu4+z4GzX8+BK4oQcnUfasc5cFUIJW1tOBYCb4GAu76+lqurq1mLgMsPAQcACyDg1jc54D55rHPgbm9vv9the2i9evWKgMsMAQcACyDg1jc34B6bY8Cd4sWLFwRcZgg4AFgAAbe+VOfATVn8CzU/BBwALICAW18qO3AEHKYg4ABgAQTc+gg4Ai4nBBwALICAW9/kgGudmDg+ZHQHQ+uc7L96fmq8yUEVxQ9HhbRtKwdpxbn9+NRJCDjMQcABwAIIuPVNDbjPd5+2w/y3vdNibCNaW6nrYcX3xziLy/tKmqoUW3fD3Lgufs/1s+AKU49/xUkIOMxBwAHAAgi49U0NuLjTpqwP0RbnvR2kikHXatkVw46bsjHo6i+7a20pz7URFyJJqUaMGb4XA86YcL0i4LA+Ag4AFkDArePi4kI+fvzYv04NuNlcKWXThZdy0blxBBzmIOAAAL+lJ0+e9OF2XIsF3EoIOMxBwAEAksAcOAIuJwQcACAJ7MARcDkh4AAASUgl4Lbb7UmLgMsLAQcASEIqAcfD7DEFAQcASMLUgBvmwDnRrpG9b6Ry+37uW5wHV6lC7PapbHavRW/+kGe7f/oZcK79MuB3KQQc5iDgAABJmBZwbYi1qn+NM94ab8LnJkTdRoyO892M1EUIOe/FFiHm4qBfa/vPSyPgMAcBBwBIwrSAOwoB1057BFacAdctOQDuEwIOcxBwAIAkzAu4x4eAwxwEHAAgCanMgSPgMAUBBwBIAjtwBFxOCDgAQBIIOAIuJwQcACAJBBwBlxMCDgCQhKkB1zotde3lMLqzdO/DsW8P3WMYP6JVIaY+bzARcJiDgAMAJGFywOmt/P2m6+e+KVuLsVUfZaqIUeaHGXDHc0pLVZZiTSGdP4j3zRBw1kihlFTOhWPh+v69l0aVcurEuGPAxWehvn37dtbiWaj5IeAAAEmYHHAhuipdhShT0oQoq2KsqTp83oUQs6JCxJkqDvUdzlUuBFyIubg7F0MuXht362LYDXFnxMThv2cMuPEO20OLgMsPAQcASMLUgHusCDjMQcABAJKQyhw4Ag5TEHAAgCSkugN3eXkpFxcX36ybmxsCLnMEHAAgCakG3H0RR8CBgAMAJCGngBufJ+DyQ8ABAJIwNeD6u1DdXnxTSR1nv7VOTN3IvrGiKyub3WvpPp3bO92/L7Xt70IttZLC1P0oEe8P/edzzYP7WcB9HXHj3TcCLk8EHAAgCZMD7gxz4I4BF2fAuRCE5zAl4O6LNwIuTwQcACAJUwNurG1b0VUzPry6hwLuZ4uAyw8BBwBIwqkB91gQcJiDgAMAJIGAI+ByQsABAJKQSsBdX1/L1dXVrEXA5YeAAwAkIZWA2263Jy0CLi8EHAAgCeNdqdwWAZeX/wHPNt8L9yTaBwAAAABJRU5ErkJggg==>

[image4]: <data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnAAAAG0CAYAAACyghqcAAA9VUlEQVR4Xu2dC7wcRZ3vSxZ3gQPyzBNCXoi8FOShIChK1HC96LLAhZW3CPhJwMgFlIcE+URAXW6QIA9BQdAEN+GdwBINSQAVZEEuD0FNeOSuH9R1RUWjSdZIXf41p8eeOn2ma6arq2a6v9/P5/uZMzU93XM69a/6nZmeitIAAAAA0FcouwEAAAAAehsCHAAAAECfQYADAAAA6DMIcAAAAAB9BgEOAAAAoM8gwHXBwMCAVkoVVvYDAAAA0CkEuC6Q8PWHZ/9N61UPdK08X/YDAAAA0CkkiC4gwAEAAEBMSBBdQIADAACAmJAguoAABwAAADEhQXQBAQ4AAABiQoLoAgIcAAAAxIQE0QUEOAAAAIgJCaILCHAAAAAQExJEFxDgAAAAICYkiC4gwAEAAEBMSBBdQIADAACAmJAguoAABwAAADEhQXQBAQ4AAABiQoLoAgleWI4DA5vYpxsAAAAsCHBdIEGj6DtwOFTelQQAAHCD2bILCHDlSIADAABwg9myC3o9wL3w0C1D2vpBAhwAAIAbzJZd0OsB7rGF1w1p6wcJcAAAAG4wW3ZBrAA3Zf899S1XztQ3/MtnGkHn9bYTjzjY3B7xoQP1+acdq2+87BwT4PZ9+y76vOnH6NkXTDeP7zhxnD7+8Kl65x3G6+nHHWq2GTtqG33x2R/Xl37mFHP/XXvt1rxvHzuEBDgAAAA3mC27IFaAm/O5T5rA9s8fPkjvuuME/dpLy/WYkVvrhV+/VL91p0lmmy3etKn+wR1X6189fqe5v+nAxnr1c4v1rDNPMve/d+tX9OTxY01g+4e/f6NpG9hkI/3ggiv1L/799ub9Pz5735Djly0BDgAAwA1myy6IFeDEt+00WR984DtMQLvgk8fpy86fpi/59Mn6sIPfo1c+MM8oAS7ZfvPNBvSL3/u2Hj1iK33T7HP1Q68HtfHbjjIBbpc3TzDbbLXFm/SCay5qPkfuP3f/zUOOXbYEOAAAADeYLbsgZoCbOeN4E9rk55Fbb2kC2w/vulaP2GqLZtv3br1S/+ie6839t0wap//64jJ95CHvM/fPOuUove3obUyA22CDDUzbhO1G639f+FX9+KLGc+S+vLtnH7tsCXAAAABuMFt2QcwA186XH72t5f6vn7ir+fP6F5a1PC4BTj5K/eVjdzTvy21yP4YEOAAAADeYLbugVwNcJyYBLn3f3ia0BDgAAAA3mC27oAoBrhclwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUEuHIkwAEAALjBbNkFBLhyJMABAAC4wWzZBQS4ciTAAQAAuMFs2QUDAwMmaKB/BzbZxD7dAAAAYEGAAwAAAOgzCHA1YN26dUYAAACoBgS4GvCHP/zBCAAAANWAAFcDCHAAAADVggAHAAAA0GcQ4AAAAAD6DAIcAAAAQJ9BgAMAAADoMwhwNYAvMQAAAFQLAlwNIMABAABUCwJcDSDAAQAAVAsCXA1YvXq1EQAAAKoBAQ4AAACgzyDAAQAAAPQZBDgAAACAPoMAVwP4EgMAAEC1IMDVAAIcAABAtSDA1QACHAAAQLUgwNUAlhEBAACoFgQ4AAAAgD6DAAcAAADQZxDgAAAAAPoMAlwNWLNmjREAAACqAQGuBvAtVAAAgGpBgKsBBDgAAIBqQYCrASwjAgAAUC0IcAAAAAB9BgEOAAAAoM8gwFWQadOm2U0tzJgxw24CAACAPoIAV0F+9atf6ccee6x5P72MyCuvvKIHBgaajwEAAED/QYCrKCNGjNCLFy82P6e/hSrtS5cuTW8KAAAAfQYBrqLMmTNH77HHHubndIDbe++905sBAABAH0KAqzBz587V69atay4jMn/+fL1+/Xp7MwAAAOgzCHAV5rXXXjPvxCXstddeqUcBAACgXyHAVZyDDjpIz5w5U0+dOlXPmjXLfhgAAAD6EAJcxZFvo2666aZG+QYqAAAA9D8dBzi5GF6p15+26oGe9Q/P/lvjNYJhxYoV+qWXXrKbAQAAoE/pOOUQ4AAAAADi0nHKIcABAAAAxKXjlEOAAwAAAIhLxymHAAcAAAAQl45TDgEOAAAAIC4dpxwCHAAAAEBcOk45BDiA/mPt2rWmJvpBea0A0HsMDAwMqddupc6L03HKIcAB9B9St1IXdq30mvIa5bUCQO8h86qPcYQ694OyG/IgwAH0HwQ4ACgKAa63UHZDHgQ4gP6DAAcARSHA9RbKbsiDAAfQfxDgAKAoBLjeQtkNeRDgAPoPAhwAFIUA11souyEPAhxA/0GAA4CiEOB6C2U35JEEOETszJhfmyfAAUBRZBzzMY5Q535QdkMe/fAOHGKvGXvAIsABQFEIcL2FshvyIMAhdm7sAYsABwBFIcD1FspuyIMAF95PnXSEuX3u/pv1fz9//5DHO1X2Y7d1at7rWPnAPH3myUc2b+3H62bsAYsABwBFIcD1FspuyKNOAW79C8v0zbPP0z9b/i1zf9m3v6znXvFZ87N0wJ8/cqu+afa55v5jC6/T//HIgpbnv/rje/VTi2/Qt391lrn//duu0iuWz83cv2z7xL1fM9v+5smFzWP84ParjHJfzvuiG76gH190vTn2o3dfa9q/efn5zWPLMebNuaB5DNn25Udv0w8umGPuP3LnNWY/a3723ZbXcf+8y5u/52svLTe/60+XNe5n7UNeh+xDXrP8jhLo5HlyfuR5ss1bJo1r3ibHSl6bfW6qbuwBiwAHAEUhwPUWym7Ioy4BTkLNlP331J89/Tg9bsxI/dcXG/fPm36MvmrWp0xge9deu+lPnniY/uhHpuhDP3iA3nLzzfRvn1rU3IeElx0mbKv3ffsu+ogPHajfu+8eJszceNk5Q/Yv207YbrTZ9sH5jaAkx5g8fqxR7st5n37coXr8tqPMsbd406bm2KIcW7aRY5x/2rHmGHJ/+7Gj9Dt239k8R9pnXzDd7OeVwZAoyuuQ58nrkED1wXfvY9omjhtjftesfcjrkH3Ia5bf8U8/+Y55npwfeV5WgJNzkLy29Lmxz30VjT1gEeAAoCgEuN5C2Q151CXAXXTGifqUjx7SvH/OtKObP0ug+vqXPtO8f+rRHza3+++9m3lnKmmXkJL8PGbk1iYUiW/ccMMh+5dtJSiln5cV4P747H0mNMn9z5/18ZZjf+71faaP8eySm0zoksef+c439HZjRui/PL90yL9f+nWIZ596VPNnOXbWPuR1SFvymuX8pJ933aVntQQ4eW3JOZDXdsuVM1uOWXVjD1gEOAAoCgGut1B2Qx51CXDyLtPxh081P69+brEJIMljY0dto7/xfxofnYqfOPoj5rZdgDvs4PeY68ES7f2n36n64V2Nj0bl49CsAJfclwCXPvYlnz655RhrVyxpbivha9vR22QGuPTrkI9LTzv+n5qPye+atY8kwCWvWc5P+nkScNMBTl5b+hw8tODKltdQdWMPWAQ4ACgKAa63UHZDHnUJcOLlM0/T79xjZ73rjhPMfQkz++y+k17+r1eYd8eS7ewAN2qbLfWd11/cEuCuuPB08w6UvIN1x3WfH7J/+1qxzTcbMB9TpgPc+w/YS286sPGwAU5u5Rj77blr8xh2+Er28+R9N5jXmbwOeV3yOiRcffGcU83vOmn7seZ3zdqHvA7ZR/o1y/Pk/Mjzsj5ClXOQvLb0uamDsQesfglwiP1k7LoOja8Ah/m69C1lN+RRpwAn/vKxO5o/yztPcjG/vY2rct2b7CPdlt5/2t8/c++QNvFPP1k8pC2tHKOb12i/Lrkv1/3Z2yUO9zrk2O2e181rq4IuxVgmBDhE/8au69AQ4MLp0reU3ZBH3QIcog9dirFMCHCI/o1d16EhwIXTpW8puyEPAhxi57oUY5kQ4PrH9LqP9mPdONyajS88dEvzWNidses6NP0Q4NL9uuwa6sThak0uW7LbRJe+peyGPAhwiJ3rUoxlQoDLV9ZClHUZk/tZ6z5mrb2YaK9taK/7mN5/etuH77i6eQx73cdfP3G3+WKRrDcpx5Zrb+XYyT6z1n2UY2St2Zhss27lEn3X1y5pfklKHlt6y+V6ydzZzW3SvzsOb+y6Dk1egMuqofR6oq41lAQm6dtJDdnrsg5XQ0kdSd93raHkGPJclxpqtz+7dqTWFn790ua15PbvNdz14C59S9kNeRDgEDvXpRjLhADXXpkcZC1EWZdRJoTh1n3MWnsxUQbidus+pvef3jb5kk/WskESrGQZH1lvUo4t603KsZP1JrPWfZzxscObS/+k12xMXqd80egjH9hf7zR5e3OsHSeOM99E33mH8eZx+3e3zxX+zdh1HZp2Ac7u40k/StYTbbd+aWJSFxKWknVDpT7sdVPT29o1lNRRsuapSw0ldSrPdamh4fZn1478sSS1dsZJRzTrOv17Jb+HfS5Fl76l7IY8CHCInetSjGVCgGuvrMuYvj/cuo/22ovp56QH4qx1H4fbtl2Ak7b0uo/Jc+Qdgbx1H+U2veRP4qc/8c/mVj5ekmPJkj5nnXKUmWil3f7d5R2K9PPxb8au69C0C3B2H7f70XDrl6afk9RFet1Q8bTjDx2yXulwNZTUUbJkVic1JNu61NBw+7N/5yMPeV/Lffv3kpolwCH2uC7FWCYEuPbKX9nJz/LxyHDrPmYt3ZOYHoiz1n1M7z+9rbwDJrdZ6z4mk1Fy7OQ5Mlnkrfsot1mTj7xzILdPL77RbD96xFbm46BkbUb7d0/WpcShxq7r0LQLcHYft/tRuh/n1ZC9buiXzvtEy3ql6W3FdA1lBTjXGkpv266Ghtuf/Tsnrzl5jv17Sc0S4BB7XJdiLBMCXL6yFqKsy5hcVJy17mPW5JOsp5geiLPWfUzvP73tCYcfPOy6j/I/lgw3Wchtu3Ufk22TNRtlbUq5L5OM/G7y7oBsLx9JHfjO3c3HPr97+h6zTfp3T/aDQ41d16FpF+DErBqy1xPtpIakb8s+5b69LqtdQ3IcqaF0CHOtoaROswKcaNdQu/3ZtSP399xtx+Zz7N+LAIfY47oUY5kQ4BD9G7uuQ5MX4NCfLn1L2Q15EOAQO9elGMuEAIfo39h1HRoCXDhd+payG/IgwCF2rksxlgkBDtG/ses6NAS4cLr0LWU35EGAQ+xcl2IsEwIcon9j13VoCHDhdOlbym7IgwCH2LkuxVgmBDhE/8au69AQ4MLp0reU3ZAHAQ6xc12KsUwIcIj+jV3XoSHAhdOlbym7IQ8CHGLnuhRjmRDgEP0bu65DQ4ALp0vfUnZDHgQ4xM51KcYyIcAh+jd2XYeGABdOl76l7IY8CHCInetSjGVCgEP0b+y6Dg0BLpwufUvZDXkQ4BA716UYy4QAh+jf2HUdGgJcOF36lrIb8iDAIXauSzGWCQEO0b+x6zo0BLhwuvQtZTfkQYBD7FyXYiwTAhyif2PXdWgIcOF06VvKbsjDR4Ar0gmKPBcxli7FWCYEOET/xq7r0DD/htOlbym7IQ8CHGLnuhRjmRDgEP0bu65Dw/wbTpe+peyGPAhw/enaFUv0wCYbmfPXjfJce5/orksxlkmZAa5Iv5J+ae8PsV+MXdehkZotaxzBVl36lrIb8iDA9adyzrr9dyvyXGzoUoxlUmaA67YmzTnp4nmIvWLsug5Nt7WOnevSt5TdkAcBrj8tEsKKPBcbuhRjmRDgEP0bu65D022tY+e69C1lN+RBgOtPi4SwIs/Fhi7FWCYEuGrZ7Tkv+lxsNXZdh4a+E06XvqXshjwIcP1pkRBW5LnY0KUYy4QAVy27PedFn4utxq7r0NB3wunSt5TdkAcBrj8tEsKKPBcbuhRjmRDgqmW357zoc7HV2HUdGvpOOF36lrIb8iDA9adFQliR52JDl2IsEwJctez2nBd9LrYau65DQ98Jp0vfUnZDHgS4/rRICCvyXGzoUoxlQoCrlt2e86LPxVZj13Vo6DvhdOlbym7IgwDXnxYJYUWeiw1dirEoF198sZ42bZrdbCDAVctuz3nR52KrIeq6l6DvhNOlbym7IQ8CXH9aJIQVeS42dClGXxx00EF6xIgRet26dc02Aly17PacF30uthqyrkOxcuVKfeSRR+qZM2cO+d3oO+F06VvKbsiDANefFglhRZ6LDV2K0SeLFy/WEydO1HPnzjX3CXDVsttzXvS52Groug5J1h+C9J1wuvQtZTfkQYDrT4uEsCLPxYYuxVgGMvjKIHzggQeWVjfd1iQBrnu7PedFn4utxqrrkMybN0/vscce5o9C+k44XfqWshvyiB3g6qavSa5ICCvyXGyYFONFF11kzqU5nylOPfXUZrv9mOD62NixY1seS+93w7/7uyGvy4fd1rOvvl1Huz3nRZ+LrbpMslUhmfsPO/jdQ84D+telbym7IQ8CXFh9TXJFQliR52JDl2Isi6VLl+opU6boacf945DX5cNu69lX366j3Z7zos/FVtN1PdwfZ0LSLrb7I6uMx+zX0+4xod1rbT4n41ygX13mDGU35EGAC6uvSS4JYejmwCYb6bUrlgw5j93qUow++fOf/6wnTZqkJ0+ebH6WY/voR1na5w6HV/qVff66UfbV7b9nkediq6HrOgbr16/Xe+21l7kUg74TTpe+peyGPOoS4F546Bb9qZOOMD8/d//NQx4Ppe8AZ7djtr77qEsx+mLWrFlmsF2wYIEZfIWyA1xZ+66SPmuwyDkv8lxsNWRdxyD5Q/Caa64xP9N3wunSt5TdkEc/BLgn7v2afmrxDfq/n7/f3J835wK9Yvnc5uP3z7tc/2z5t1q2/c2TC/XDd1xt2pLQ9IPbr9KP3HmNeb1rfvZd89j3b7uquS/7OGVIgIuj7z7qUoxFkXXgjjnmGLvZQICLr88aLHLOizwXWw1R16FZtWqVnjFjhp46daq5/CINfSecLn1L2Q159EOAm7DdaL3DhG1N6DriQwfq8087Vr9l0jjz2PoXlun37ruHHjdmpL79q7Oa2z44f05zm8cWXmecPH6snn3BdPN6X3k94Mm+5Lmy3Y2XndNyHPs1+JIAF0fffdSlGMuEABdfnzUo+0F3fV4OkTZ2XZdBuz8E5VxS62F06VvKbsijHwLc9mNHmdvPnXGiHjNyaz1l/z2Nzy65SV/0elvWtvJOW1aA+8vzS5u/b3pfb9xww+Zzy7TKAS45x//xyAJ98dkfH/KYvb0o5/2Pz943pN23vvuoSzGWSd0DXNK/QvWfLH3WYD+c817R1xiaZey6Dg39LpwufUvZDXn0Q4BLgtglnz5ZH3bwe/TKB+YZ5a8weUdNHlv93GL9+KLrm9v+8K5r9Y4TGz/Lx6RZAS69LzF5bpn6Gnx8Th628q7mzbPPa34sLe9IyjuUS+bONseV8yyPP7hgjnl83col+q6vXWLOs5xj2UY+rk4eW/j1S81j6X399cVl5r78Dotu+IL5Of1xtm9991GXYiyTfg1wr/74XnOZgrxbLvftf/Ok79nbpi+HkL6V9K90/0lfWmEfpwx91mCZ57xq+hpDs4xd16Gh34XTpW8puyGPfgpw4hUXnq7323NXvd2YEc02+XnXHScMCWEnHH6wnrT9WD39uEObAU7a33/AXvrJ+24w+5J34eT5d1z3eQLc68p52OXNE8zPD8yfY77wMevMk8z97936FXMe5V0Pub/0lsvN7QH7vNXcXnvJmeYcJ+f6ptnntjyW3lfybyG/g7yD8tadJplQ9/NHbtVbvGnTIa+rqL77qEsxlkm/Bjh5Z/yXj91hfrb/zdN979LPnNKybfrddOk7Wf1H7su+5A+79HPL0mcNlnnOuzX50ldym5j+QphtmdcPJ/oaQ7OMXdeh6cV+V1Vd+payG/LohwBn+/Kjt5l30pL76Z9tf//MvUPa0spf/O2e71tfg4/PySOtfCR9ykcPaWl7aMGV+pAp++mJ48aYCXT8to2Pmp/5zjfM7ac/8c/mVgJaOsCd8fogn34sva9kH8kEbH+cbb+uovruoy7FWCb9HOCSn+1/c7vvpbfNC3D2pRXp55alzxos85x3a3KOsy6HSB5LK2NpiI+zfY2hWcau69D0Yr+rqi59S9kNefRjgOtnfQ0+PiePtD+653o9esRW5md51+zCT52gjzzkfeb+Wacc1TJ4JwFOvkDy26cWmbCWDnDyDp48JtvIY+l9bTt6G3P7hje8Qf/XE3frEVttYYL5o3dfq0duveWQ11VU333UpRjLpAoBzv43T/e9qe/Zp2VbWW/tP390p/7kiYe1BLh0/5H7sq8//WQxAW5QCbTpL3nJZSXHHz5V77zDePPJhFz3+47ddzZ/UMmXw+SSh498YH/zx1dyjpPbfXbfyTy20+TtTVt6X/K4XM4i+5QviMkxZX9yuYT9morqawzNMnZdh6asfodDdelbym7IgwAXVl+Dj8/Jw/bymafpd+6xs/lY+qfLvmUG//333k2fN/0YvezbXx4S4OTLJWNHbWPCWTrAJY/tuduO5rH0vmRC/t3T95iPszcd2HjIx9n2ayqq7z7qUoxlUoUAl/VvnvQ9uRwiva1cDrH5ZgMmIKQDXLr/pC+tIMA1lI+k5WNq+WNKPpZOXw4h51De+fz1E3eZyyHkvlz2kDzXDnD2pRLpfcmtfJIh78DJNa/JNa7y0bjvb4z6GkOzjF3XoSmr3+FQXfqWshvyIMCF1dfg43PyqIO++6hLMZZJvwa4KumzBss65/IOmdwmX/KSdzglpMmlDPKuW/qPMXlX/NxpRzefawc4efdTbp9efKNpS+9L2pMAJ182S385jADXu5TV73CoLn1L2Q15EODC6mvw8Tl51EHffdSlGMuEABdfnzVY1jmXkCUfacq7ZvKxdPpyCAlsdoCTd+rkcghpswOcfalEel9y+9pLy83H2bICgHw0Lm3y0bh8pG2/riL6GkOzjF3XoSmr3+FQXfqWshvyIMCF1dfg43PyqIO++6hLMZYJAS6+PmuwzHOe/pKWfNEgCVfDKa9DlmGx25PH2u0rCWtZj/nS1xiaZey6Dk2Z/Q5bdelbym7IgwAXVl+Dj8/Jow767qMuxVgmBLj4+qxBzrm7vsbQLGPXdWjod+F06VvKbsiDABdWX4OPz8mjDvruoy7FWCYEuPj6rEHOubu+xtAsY9d1aOh34XTpW8puyIMAF1Zfg4/PyaMO+u6jLsVYJgS4+PqsQc65u77G0Cxj13Vo6HfhdOlbym7IgwAXVl+Dj8/Jow767qMuxVgmBLj4+qxBzrm7vsbQLGPXdWjod+F06VvKbsiDABdWX4OPz8mjDvruoy7FWCYEuPj6rEHOubu+xtAsY9d1aOh34XTpW8puyIMAF1Zfg4/PyaMO+u6jLsVYJgS4+PqsQc65u77G0Cxj13Vo6HfhdOlbym7IgwAXVl+Dj8/Jow767qMuxVgmBLj4+qxBzrm7vsbQLGPXNdQbZTfkQYALq6/Bx+fkUQd999HYAz0BLr4+a5Bz7q6vMTTL2HUN9UbZDXkQ4MLqa/DxOXnUQd99NPZAT4CLr88a5Jy762sMzTJ2XYMbU6dO1SeddJLd3PcouyEPAlxYfQ0+PiePOui7j8Ye6Alw8fVZg5xzd32NoVnGrmvI5+6779YjR47UG2+8sX7xxRfth/saZTfkQYALq6/Bx+fkUQd999HYAz0BLr4+a5Bz7q6vMTTL2HUN+ey999761Vdf1U8//bQJcVVC2Q15EODC6mvw8Tl51EHffTT2QE+Ai6/PGuScu+trDM0ydl1De15++WX98MMPN8a/163ax6jKbsiDABdWX4OPz8mjDvruo7EHegJcfH3WIOfcXV9jaJax6xqGR4LbZpttZn5OApxQpRCn7IY8CHBh9TX4+Jw86qDvPhp7oCfAxddnDXLO3fU1hmYZu65heHbYYQe9bt06u9kEuKqEOGU35EGAC6uvwcfn5FEHfffR2AM9AS6+PmuQc+6urzE0y9h1Ddn8/ve/1wsXLrSbDS+88EJlroVTdkMeBLiw+hp8fE4eddB3H4090BPg4uuzBjnn7voaQ7OMXdeQzSWXXGI3tSAhrgoouyEPAlxYfQ0+PiePOui7j8Ye6Alw8fVZg5xzd32NoVnGrmuoN8puyIMAF1Zfg4/PyaMO+u6jsQd6Alx8fdYg59xdX2NolrHrGtww418F/52U3ZAHAS6svgYfn5NHHfTdR2MP9AS4+PqsQc65u77G0Cxj1zW4QYAbhAAXVl+Dj8/Jow767qOxB3oCXHx91iDn3F1fY2iWsesa3CDADUKAC6uvwcfn5FEHfffR2AM9AS6+PmuQc+6urzE0y9h1DW6sWbPGWDWU3ZAHAS6svgYfn5NHHfTdR2MP9AS4+PqsQc65u77G0Cxj1zXUG2U35EGAC6uvwcfn5FEHfffR2AM9AS6+PmuQc+6urzE0y9h1DfVG2Q15EODC6mvw8Tl51EHffTT2QE+Ai6/PGuScu+trDM0ydl1DvVF2Qx4EuLD6Gnx8Th510HcfjT3QE+Di67MGOefu+hpDs4xd1+DG6tWrjVVD2Q15EODC6mvw8Tl51EHffTT2QE+Ai6/PGuScu+trDM0ydl2DG2b8q+C/k7Ib8iDAhdXX4ONz8qiDvvto7IGeABdfnzXIOXfX1xiaZey6BjcIcIMQ4MLqa/DxOXnUQd99NPZAT4CLr88a5Jy762sMzTJ2XYMbLCMyCAEurL4GH5+TRx303UdjD/QEuPj6rEHOubu+xtAsY9c11BtlN+RBgAurr8HH5+RRB3330dgDPQEuvj5rkHPurq8xNMvYdQ31RtkNeRDgwupr8PE5edRB33009kBPgIuvzxrknLvrawzNMnZdQ71RdkMeBLiw+hp81q5Yogc22cice8xXzpWcM/s8dmvsgZ4AF18CXBx9jaFZxq5rcINlRAbxEeAQ62bsgZ4AF18CXBwJcGDGvwr+Oym7IQ8CHGLnxh7oCXDxJcDFkQAHBLhBCHCInRt7oC8zwPHRvLtyruzzh/1r7LoGN1hGZBACHGLnxh7oywxwiHU1dl1DvVF2Qx4EOMTOjT3QE+AQ/Ru7rqHeKLshDwIcYufGHugJcIj+jV3XUG+U3ZAHAQ6xc2MP9AQ4RP/Grmtwg2VEBiHAIXZu7IGeAIfo39h1DW6Y8a+C/07KbsiDAIfYubEHegIcon9j1zW4QYAbhACH2LmxB3oCHKJ/Y9c1uMEyIoMQ4BA7N/ZAT4BD9G/suoZ6o+yGPOoW4Ir+H6I+/z9N7F9jD/QEOET/xq5rqDfKbsijbgFOCrTb39cUN5Mmroo/0BPgEP0bu66h3ii7IQ8CnLsEOEyMPdAT4BD9G7uuwQ2WERmEAOcuAQ4TYw/0BDhE/8aua3DDjH8V/HdSdkMeBDh3CXCYGHugJ8A1lFru9jx0Ow5gdY1d1+AGAW4QApy7BDhMjD3QE+AaEuDQp7HrGtwgwA1CgHOXAIeJsQd6AlxDAhz6NHZdQ71RdkMeBDh3CXCYGHugJ8A1JMChT2PXNdQbZTfkQYBzlwCHibEHegJcQwIc+jR2XUO9UXZDHgQ4dwlwmBh7oCfANSTAoU9j1zXUG2U35EGAc5cAh4mxB3oCXEMCHPo0dl2DG2b8q+C/k7Ib8iDAuUuAw8TYAz0BriEBDn0au67BDQLcIAQ4dwlwmBh7oCfANSTAoU9j1zW4QYAbhADnLgEOE2MP9AS4hgQ49GnsuoZ6o+yGPAhw7hLgMDH2QE+Aa0iAQ5/GrmuoN8puyIMAh+0sMkFW2dgDPQGuYZH+yTiAtrHrGuqNshvyIMBhO4tMkFU29kBPgGtYpH8yDqBt7LqGeqPshjwIcNjOIhNklY090BPgGhbpn4wDaBu7rsENM/5V8N9J2Q15EODC+cqTC/VRhxzU0nb7V2fplx+9bci2vWKRCbLKxh7oCXANpX+iu2tXLBlyDvFvxq5rcIMANwgBrjNf/fG9+uE7rm7uK2mfN+cCfc+NX2ze//5tV5m25DlPLb5Bv/T9b5tjf/srF5r2VT+Yr/d9+y76J0u/ae4vveVyfeNl5+i/vrjM3H980fX65tnn6QcXzGnu195m3col+u6vXaJ//8y9ze3kuCuWz2153d0qr5egMNTYAz0BriH9013TZzlXbY1d1+AGAW4QAlxnPnLnNfotk8aZnx9beJ25/emyb+nzph+jpx37j/p3T99j2t677x76/NOONWFLnrPDhG318w/eYo592MHvMdt88ZxT9ZcvPN0EuD8+e58+/vCp+sJPnaCnH3eoeXz7saP0jI8drsdvO8rcz9pmr7fuqD/8/nfp3XeerLfe8k36iA8daI6bvMaiMkFmG3ugJ8A1pH+6S4DLN3Zdgxtr1qwxVg1lN+RBgOvMrAD3X0/crTffbMCEJ7n/uTNO1FP239P4xg031LdcOdO0//yRW82x//ED+5t34d6339tNe/IO3FmnHKUPmbJfM7Alt8985xvN49vbSFiT2//38HwT4MaM3Lp57GeX3DTk9XcqE2S2sQd6AlzDmP1TLoe4+OyPN+/L5RD2JRK9JAEu39h1DfVG2Q15EOA684d3Xat3nNgIcPIxadL+9OIb9ZfO/YT+5uXn60s+fbJe+cC8pg8tuNJskwQ4CXTyLtz1XzjbtEuA+863LtM3zT7XhLUknE0eP9bcJgEua5vTT/gnc/vj795kApzsNzmuj+tdYk6QvWzsgZ4A1zBm/5Rjyx9uyf399txVb7DBBkO26xUJcPnGrmuoN8puyIMA17knHH6wnrT92ObHmL99apHe5c0T9J677aj/80d3mjZ5J0wG9Duu+7x5107akgAnPyfv1okS4ORj2APfubvef+/d9MAmG5mPYu0Al7XNdZeeZV6LfHwrx7ziwtPNcbcbM2LI6+7GmBNkLxt7oCfANSzaP9Pvpku9SY3JH0dST6O22dI8lr4sQWp5wnajzbWrcmz5gy3Zl1wOIXUplzrIH3lyqcPOO4xv7v8du+9s9i37ytpG/uCSyyHOPPlI88eYtMmlGHJcuRTDfu2dSoDLN3ZdQ71RdkMeBLj+duTWW5rQJpPANRf/7yGPF7XoBFlVYw/0BLiGRfunHeDkcggJWhLabrt2lrkcIn1Zgrx7LtemJseWW7kcIrkMQgKc3Mq77nKpw8RxY5r7l1v5Yyz548reJnk3XZQAZ1+KYb/2TiXA5Ru7rsENM/5V8N9J2Q15EOD62+fuv1lf+plTWr6p6tOiE2RVjT3QE+AaFu2f6cshkne8X3tpubkcQoKbXA6RvixBQlcS+pJxRB6XsCU/S4CTSx1Gj9jKhLXk8ol0gNt29DaZ21zwyeOar0sCnH0phv3aO5UAl2/sugY3CHCDEOCwnUUnyKoae6AnwDUs2j/lcgi5jk0uh5AAJ5dDyCUIcjmEfEwq26QvS0h/iSkZR+Tduv950L7mZwlw8jHsuDEjzaUO8u10udTBDnBZ28jjcjnEmyduZ8Kj3JdbOa5cimG/9k4lwOUbu67BDQLcIAQ4bGfRCbKqxh7oCXANffRPWUMxr00W2/7L80uHbDec619YZrTb222TfAwrSoBMtunkuO0kwOUbu67BDZYRGYQAh+30MUFW0dgDPQGuYdX6p1wOIV9ssNt9SIDLN3ZdQ71RdkMeBDhsZ9UmSF/GHugJcA3pn+4S4PKNXddQb5TdkAcBDtvJBJlt7IGeANeQ/ukuAS7f2HUN9UbZDXkQ4LCdTJDZxh7oCXAN6Z/uEuDyjV3X4Mbq1auNVUPZDXkQ4LCdTJDZxh7oCXAN6Z/uEuDyjV3X4IYZ/yr476TshjwIcNhOJshsYw/0BLiG9E93CXD5xq5rcIMANwgBDtvJBJlt7IGeANeQ/ukuAS7f2HUNbrCMyCAEOGwnE2S2sQd6AlxD+qe7BLh8Y9c11BtlN+RBgMN2MkFmG3ugJ8A1pH+6S4DLN3ZdQ71RdkMeBDhsJxNktrEHegJcQ/qnuwS4fGPXNdQbZTfkQYDDdjJBZht7oCfANaR/ukuAyzd2XYMbLCMyCAEO28kEmW3sgZ4A15D+6S4BLt/YdQ1umPGvgv9Oym7IgwCH7WSCzDb2QE+Aa0j/dJcAl2/sugY3CHCDEOCwnUyQ2cYe6AlwDemf7hLg8o1d1+AGy4gMQoDDdjJBZht7oCfANaR/ukuAyzd2XUO9UXZDHgQ4bCcTZLaxB3oCXEP6p7sEuHxj1zXUG2U35EGAw3YyQWYbe6AnwDWkf7pLgMs3dl1DvVF2Qx4EOGwnE2S2sQd6AlxD+qe7BLh8Y9c1uMEyIoMQ4LCdTJDZxh7oCXAN6Z/uEuDyjV3X4IYZ/yr476TshjwIcNhOJshsYw/0BLiG9E93CXD5xq5rcIMANwgBDtvJBJlt7IGeANeQ/ukuAS7f2HUNbhDgBiHAYTuZILONPdAT4BrSP90lwOUbu66h3ii7IQ8CHLaTCTLb2AM9Aa4h/dNdAly+sesa6o2yG/IgwGE7mSCzjT3QE+Aa0j/dJcDlG7uuod4ouyEPAhy2kwky29gDPQGuIf3TXQJcvrHrGtxgGZFBCHDYTibIbGMP9AS4hvRPdwlw+caua3DDjH8V/HdSdkMeBDhsJxNktrEHegJcQ/qnuwS4fGPXNbhBgBuEAIftZILMNvZAT4BrSP90lwCXb+y6BjcIcIMQ4LCdTJDZxh7oCXAN6Z/uEuDyjV3XUG+U3ZAHAQ7byQSZbeyBngDXkP7pLgEu39h1DfVG2Q15EOCwnUyQ2cYe6AlwDemf7hLg8o1d11BvlN2QBwEO28kEmW3sgZ4A15D+6S4BLt/YdQ31RtkNeRDgsJ1MkNnGHugJcA3pn+4S4PKNXdfghhn/KvjvpOyGPAhw2E4myGxjD/QEuIb0T3cJcPnGrmtwgwA3SN0CHKIPYw/0BLiGBDh3CXD5xq5rcIMANwgBDrFzYw/0BLiGBDh3CXD5xq5rcGPNmjXGqqHshjwIcIidG3ugJ8A1HNhkIzN+oZtrVywZcg7xb8aua6g3ym7IgwCH2LmxB3oCHKJ/Y9c11BtlN+RBgEPs3NgDPQEO0b+x6xrqjbIb8iDAIXZu7IGeAIfo39h1DW6Y8a+C/07KbsiDAIfYubEHegIcon9j1zW4QYAbhACH2LmxB3oCHKJ/Y9c1uEGAG4QAh9i5sQd6Ahyif2PXNbjBMiKDEOAQOzf2QE+AQ/Rv7LqGeqPshjwIcIidG3ugJ8Ah+jd2XUO9UXZDHgQ4xM6NPdAT4BD9G7uuod4ouyEPAhxi58Ye6AlwiP6NXdfgxurVq41VQ9kNeRDgEDs39kBPgEP0b+y6BjfM+FfBfydlN+RBgEPs3NgDPQEO0b+x6xrcIMANQoBD7NzYAz0BDtG/sesa3GAZkUHWrl2rBwY2MSEOEd2V2omFHNt+PYhY3Jh1DfWm4wAHAAAAAHEhwNWAdevWGQGgulT1Oh8AyIYAVwMY2AGqD3UOkA3LiEDfwsAOUH2qOkkBFKWqcyABrgYwsAMAQF0hwAEAAAD0GSwjAgAAAAA9AQEOAKACVPVdBgDIhgBXAxjYAapPVa/zAYBsCHA1gIEdoPpQ5wDZVPWLfAS4GsDADlB9qHOAbKpaGwS4GlDVvz4AAADyIMABAAAA9BkEOAAAAADoCQhwAAAVgG+bA9QLAlwNYGAHqD5V/ZgIALIhwNUABnaA6kOdA2RT1S/yEeBqAAM7QPWhzgGyqWptEOBqQFX/+gAAAMiDAAcAAADQZxDgAAAAAKAnIMABAAAA9BkEuBrAMiIA1aeqHxMBQDYEuBrAwA5QfahzgHpBgKsBDOwA1Yc6B8imqrVBgKsBVe28APA3WC4IIJuqzoEEOAAAAKgsBDgAAAAA6AkIcAAAAAB9BgGuBrCMCED1qerHRACQDQGuBjCwA1Qf6hygXhDgKsjFF1/cct8e2FetWpV6FACqgF3nANCgqrVBgKsg06ZNa7lvd94ZM2akHgWAKsAyIgDZ2HNgVSDAVZSDDjrIbjLMmjVLT5061W4GAACoJAQ46CtGjBihFy9ebDeb9qVLl9rNAAAAlaSqX+QjwFWUdevW6YkTJ7a0rV+/Xi9YsKClDQAAAPoPAlyFmTt3rglyCfPnzzchDgCqR1XfZQCAbAhwFUc+Mv3FL36hFy1aZH4GgGpS1et8ACAbAlzFkS8znHPOOXrKlCnmCwwAUE0IcADZVPUb2gS4DNauXauVUl6VfcbisMMOG7K0CAD0N3ZN25PUMccck3oUoL5U9Y8bAlwG5h/72X/TetUDXpR9VbHzAEBc2i0XBAANCHA1ggAHAP1A1nJBf/7zn7neFSBFVb/go+wGIMABQH+QtVzQpEmTWC4IoAYouwEIcADQPyTLBSXvMkyePJnlggBqgLIbgAAHAP1Ferkg+QgVAKqPshuAAAcA/UV6uSAAaMX+hnZVUHYDEOAA6ows+TMwMDBkKaBulP2EWkJIlguaPn263QxQe/gWao0gwAHUF6lVCV92HXej7CdU7a9cuVKvWrXKbgaoPQS4GkGAA6gv/RrgACAblhGpEQQ4gPpCgAOAfkDZDUCAA6gzBDgA6AeU3QAEOIA6Q4ADgH5A2Q1AgAOoMwQ4gGrBMiI1ggAHUF8IcADVwszpFaxDZTeA/wCHfiQIQwgIcADVggBXIwhwvSkBDkJAgAOoFiwjUiMIcL0pAQ5CQIADgH5A2Q3QPwHuPx5ZoC8+++N6/QvL9Bs33HDI450q+/jjs/cNaQ9l3u9BgIMQEOAAoB9QdgPkBzgJGvfPu1z/bPm3zP3XXlqul337y/qnyxr3H190vf75I7fqR+++Vj+28Dr9zcvPH7KPJ+79mn5q8Q36v5+/39yfN+cCvWL53Obj6f0n2/7myYX64TuuNm0mzLzuD26/Sj9y5zVmovj1E3eb4940+1yzjRxbQl6yz+/fdlXzGPJceZ03zz6v+bjsY9ENX2jel21kH3Ov+Kz5XaRtzc++q2+87Bz91xeXmfsvP3qbuV393GL9vVu/0vK7S7v87unXIL9n8rNsK89/cMEccz/5PeQYyTZpCXAQAgIcAPQDym6A/AC3y5snmADzwPw5+tLPnKL3fttbTPvzD96iNx3Y2LyL9KvH79RnnXJU8zkSyNL72GCDDfQvH7tDL/z6pfqtO00ybRJ81q5You+47vMt+0+2lYDzlknjzLYSrMTJ48fqvzy/1EwUcl+OK4+/+x1vM7dv22myObYcQ/Ypx9jiTZuabZN3u2Qfciv7SL8DJ9v8w9+/0fy86gfzTeCadeZJLc/ZfLMBE7j2ffsuevE3L2v53dOvIf17yvHl95Rtf/3EXXrpLZe3/B7p85SWAAchIMABVAuWEakReQHulI8e0nL/7FP/FtQkiEzYbrT5+fNnfbzZnn5nS9x+7Chz+7kzTtRjRm6tp+y/p/HZJTfpi15vy9rWJcAlzzn16A+b2/333s0cO30MCU6y7fhtG/vdbswIc5sV4HbfeXLzvgS4hxZcqQ+Zsl/zuQfs81a94OqL9LajtzEBMf27p19D+veU48vvmezjme98w7wGAhz0AgQ4gGph5vQK1qGyGyA/wI0esZV+5cmF+tpLztRT37OPCS+/+b8LTVDaZqvNm+9OZQW43z19j7lNgtgP77pWj9hqC/OzfOz4p58s1j+65/qW/SfbysefA5tsZH7+5ImHNQOcfIT7hje8oSXAfeLoj5jbJMDJMeTjSjnGyK23bD5XtpHXL7eyj/964m795H03mNcp28i7f/LYnddfrC+ccbw+8pD3tTxHAp/s77n7bzb30797+jWkf0/ZXn7PZFsJcLK/5PdIfgdbAhyEwGeAw+GlniEUBLgakRfgLp95mnnHaNcdJ+iVD8zTXzznVD121DZ60vZj9fJ/vaJtgJMgJLdJKBOvuPB0vd+euzbfCRPT+09ve8LhB5vjTD/u0JYQ9v4D9tK3XDmzuZ0d4OQY8g6Y7Fc+os0KcLIP+Qh41DZbmtcp20jo2mf3nfTEcWPMNX7jxow0+5QgmYTRj/2v/9E87nABLv17yvHT2yYBLnkNEiCT/aVlwIcQEODCSD1DKAhwNSIvwNXFdMgbzjNOOqL5EW/ZMuBDCAhwYaSeAYqh7AYgwCXKFwzS7+plKe+0/WTpN4e0lyEDPoSgHwPcp17/Q0puk2+1FzG5HKKdso0cS44rnxKcefKRQ7bJk3oGKIayG4AA16sy4EMIXAOcLCcky/CklxOSJXeS5YSkv6aX9bGX1Hn1x/eaJYJu/+osE4ZkmZ/0Mjvp/ae3leWEkm1kGaHkVr7kJJdLyHHlC0dy3KyljNLHkW3l2thkOaGspXzk2PbxZBs5VnJcucxD9iP7TS6tyJN6BiiGshuAANerMuBDCFwCnIQr+Ub1Z08/zlwXKt/A/uC799HnTT/GXC8q20hwetdeu5kvHH30I1OMW26+WXMfEnzkW9uyBI8Epvfuu4c+/7RjzTqL8nh6/+ltH5zfWDdRTC5xkNvZF0xvXhsr3/CW48qSPYd+8ABz7N8+tchsmz6ObPuO3XfWMz52uGmTfcjvLl+iSr9O+3iyjRxL7icBTo4pv79cQ+sS4qhngGIouwEIcL0qAz6EwCXAyVI/6eWEzpl2dMtyQvIu13DL+iRtEnzSywmll/mRZXbS+09vmxWo5FaW4ZFvhctxh1vKyD7Ogqs/13x8uKV8so6XLDmUDnASBGVdyNuundXy/OGkniEUZk6vYF9TdgMQ4HpVBnwIgUuAk6V+Ro/Yyvwsy/3I+ojJN6klKMlC1cMt6yO38k3r9LqOssxO8r+aJMvspPef3laWE/rPHzUW7E4HOPkIV5YBSn/5yA5w9nG+d+uVzcftpXySb4NnHS9Zcigd4I79pw+Yx+Sb8hIUk/0OJ/UMoSDA1QgCXG/KgA8hcAlwoiwn9M49djbL/ch9WU5IltyRZX7kfrsAJx8zpkOZKMv8pJfZSe/f3lbe6ZLjpAOc3MoyQO0CnH2c9Gu0l/KR1zjc8ZIlh9IBTvYp/0uNfMybBL52Us8QCgJcjSDA9aYM+BAC1wCHxaSeAYqh7AYgwPWqDPgQAgJcGKlngGIouwEIcL0qAz6EgAAXRuoZoBjKbgACXK/KgA8hIMCFkXoGKIayG4AA16sy4EMICHBhpJ4hFGZOr2BfU3YDEOB6VQZ8CAEBLozUM4SCAFcjCHC9KQM+hIAAF0bqGUJBgKsRBLjelAEfQkCACyP1DKFYs2aNsWoouwEIcL0qAz6EgAAXRuoZoBjKbgACXK/KgA8hIMCFkXoGKIayG4AA16sy4EMICHBhpJ4BiqHsBiDA9aoM+BACAlwYqWcIhZnTK9jXlN0ABLhelQEfQkCACyP1DKEgwNUIAlxvyoAPISDAhZF6hlAQ4GoEAa43ZcCHEBDgwkg9QyhYRqRGEOB6UwZ8CAEBLozUM0AxlN0ABLhelQEfQkCACyP1DFAMZTcAAa5XZcCHEBDgwkg9AxRD2Q1AgOtVGfAhBAS4MFLPEIrVq1cbq4ayG4AA16sy4EMICHBhpJ4hFGZOr2BfU3YDEOB6VQZ8CAEBLozUM4SCAFcjCHC9KQM+hIAAF0bqGaAYym4AAlyvyoAPISDAhZF6BiiGshuAANerMuBDCAhwYaSeAYqh7AYgwPWqDPgQAgJcGKlngGIouwEIcL0qAz6EgAAXRuoZoBjKbgACXK/KgA8hIMCFkXoGKIayG4AA16sy4EMICHBhpJ4BiqHsBiDA9aoM+BACAlwYqWeAYii7AQhwvSoDPoSAABdG6hmgGMpuAAJcr8qADyEgwIWRegYohrIbgADXqzLgQwgIcGGkngGKoewGIMD1qgz4EAICXBipZ4BiKLsBCHC9KgM+hIAAF0bqGaAYym4AAlyvyoAPISDAhZF6BiiGshuAANerMuBDCAhwYaSeAYqh7AYgwPWqDPgQAgJcGKlngGIouwEIcL0qAz6EgAAXRuoZoBjKbgACXK/KgA8hIMCFkXoGKIayG4AA16sy4EMICHBhpJ4BiqHsBiDA9aoM+BACAlwYqWeAYii7AQhwvSoDPoSAABdG6hmgGMpuAAJcr8qADyEgwIWRegYohrIbgADXqzLgQwgIcGGkngGKoewGIMD1qgz4EAICXBipZ4BiKLsBCHC9KgM+hIAAF0bqGaAYym4AAlyvyoAPISDAhZF6BiiGshuAANerMuADAAA0UHYDEOB6VQIcAABAA2U3AAGuVyXAAQAANFB2AxDgelUCHAAAQANlNwABrlclwAEAADRQdgMQ4HpVAhwAAEADZTcAAa5XJcABAAA0UHYDAAAAAPQ2BDgAAACAPoMABwAAANBnEOAAAAAA+gwCHAAAAECf8f8BAreeDiHS+GkAAAAASUVORK5CYII=>