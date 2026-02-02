# Use Case Specification
## Project: Apartment Management System
### Group: 5

This document lists all system use cases derived from the Requirements Definition Specification (RDS).

## 1. Actors
*   **Administrator (Admin)**: Superuser with full access to configuration and management.
*   **Staff**: Operational personnel handling daily tasks (security, reception, maintenance).
*   **Resident**: Tenants or owners living in the apartments.
*   **System**: Automated background processes.

## 2. Use Case List

### 2.1 User Authentication & Personal
| ID | Use Case Name | Primary Actor | Type | Description |
|:---|:---|:---|:---|:---|
| UC-01 | User Login | All Users | **Primary** | Authenticate into the system. |
| UC-02 | View Dashboard | All Users | **Primary** | View role-specific summary and notifications. |
| UC-03 | Update Profile | All Users | Secondary | Edit personal contact information. |
| UC-04 | Change Password | All Users | Secondary | Update security credentials. |

### 2.2 System Administration
| ID | Use Case Name | Primary Actor | Type | Description |
|:---|:---|:---|:---|:---|
| UC-05 | Manage Users | Admin | **Primary** | Create, update, block Staff and Admin accounts. |
| UC-06 | Manage Roles | Admin | Secondary | Configure user permissions (if dynamic). |
| UC-07 | Manage Service Types | Admin | Secondary | Define available services (Water, Electric, Parking). |
| UC-08 | Manage Service Prices | Admin | **Primary** | Update unit prices for services. |
| UC-09 | Manage Amenities | Admin | Secondary | Setup facilities (Gym, Pool, BBQ). |

### 2.3 Apartment & Resident Management
| ID | Use Case Name | Primary Actor | Type | Description |
|:---|:---|:---|:---|:---|
| UC-10 | View Apartment List | Admin, Staff | **Primary** | Monitor status of all apartment units. |
| UC-11 | Manage Residents | Admin, Staff | **Primary** | Add new residents, update info, move-out processing. |
| UC-12 | Manage Resident Cards | Staff | **Primary** | Issue, lock, or delete access cards. |
| UC-13 | Manage Vehicles | Staff | Secondary | Register resident vehicles for parking. |
| UC-14 | Register Visitor | Staff, Resident | **Primary** | Register guest entry/exit or pre-register. |

### 2.4 Contract Management
| ID | Use Case Name | Primary Actor | Type | Description |
|:---|:---|:---|:---|:---|
| UC-15 | Create Contract | Admin | **Primary** | Draft and finalize lease agreements. |
| UC-16 | Terminate Contract | Admin | Secondary | Process early termination or expiration. |
| UC-17 | View Contracts | Resident | Secondary | View own lease details and terms. |

### 2.5 Service & Billing Management
| ID | Use Case Name | Primary Actor | Type | Description |
|:---|:---|:---|:---|:---|
| UC-18 | Record Meter Readings | Staff | **Primary** | Input monthly usage for variable services. |
| UC-19 | Register Apt Services | Staff | Secondary | Sign up apartment for fixed services (Internet). |
| UC-20 | Generate Invoice | Admin, System | **Primary** | Calculate monthly bill based on usage/fees. |
| UC-21 | View Invoices | Resident | **Primary** | View monthly payment obligations. |
| UC-22 | Process Payment | Staff | **Primary** | Record cash/transfer payment manually. |
| UC-23 | View Payment History | Resident | Secondary | Review past transactions. |

### 2.6 Communication & Operations
| ID | Use Case Name | Primary Actor | Type | Description |
|:---|:---|:---|:---|:---|
| UC-24 | Submit Request | Resident | **Primary** | Create ticket for repair/complaint. |
| UC-25 | Process Request | Staff | **Primary** | Update status, assign staff, resolve ticket. |
| UC-26 | Post Announcement | Admin | **Primary** | Publish building-wide news. |
| UC-27 | View Notifications | All Users | **Primary** | Receive alerts on bills, requests, news. |
| UC-28 | Send Message | Resident, Staff | Secondary | Chat feature for direct communication. |
| UC-29 | Book Amenity | Resident | Secondary | Reserve time slot for facilities. |
| UC-30 | Manage Parcels | Staff | Secondary | Receive and notify residents of packages. |

