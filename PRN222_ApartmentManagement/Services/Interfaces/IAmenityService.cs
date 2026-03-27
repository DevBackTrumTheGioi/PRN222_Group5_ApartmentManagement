using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IAmenityService
{
    Task<IReadOnlyList<Amenity>> GetManagerAmenitiesAsync(string? search, bool? isActive, bool? requiresBooking);
    Task<IReadOnlyList<AmenityType>> GetAmenityTypesAsync();
    Task<Amenity?> GetAmenityByIdAsync(int amenityId);
    Task<(bool Success, string Message, Amenity? Amenity)> SaveAmenityAsync(Amenity amenity);
    Task<(bool Success, string Message)> DeleteAmenityAsync(int amenityId);

    Task<IReadOnlyList<Amenity>> GetResidentAmenitiesAsync(string? search = null);
    Task<IReadOnlyList<AmenityAvailabilitySlotDto>> GetAvailabilitySlotsAsync(int amenityId, DateTime bookingDate);
    Task<(bool Success, string Message, AmenityBooking? Booking)> CreateBookingAsync(
        int residentId,
        int apartmentId,
        int amenityId,
        DateTime bookingDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int participantCount,
        string? notes);
    Task<IReadOnlyList<AmenityBooking>> GetResidentBookingsAsync(int residentId, string? status = null);
    Task<IReadOnlyList<AmenityBooking>> GetManagerBookingsAsync(
        string? searchTerm,
        string? status = null,
        int? amenityId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<(bool Success, string Message)> CancelBookingAsync(int bookingId, int residentId);
    Task<(bool Success, string Message)> UpdateBookingStatusAsync(int bookingId, string status);
    Task<bool> ResidentHasApartmentAsync(int residentId);
}
