using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN222_ApartmentManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddResidentApartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: tạo bảng chỉ khi chưa tồn tại
            // (bảng có thể đã được tạo từ migration trước đó đã bị xóa khỏi code)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ResidentApartments')
                BEGIN
                    CREATE TABLE [ResidentApartments] (
                        [ResidentApartmentId] int NOT NULL IDENTITY(1,1),
                        [UserId] int NOT NULL,
                        [ApartmentId] int NOT NULL,
                        [ContractId] int NOT NULL,
                        [ResidencyType] nvarchar(50) NOT NULL,
                        [IsActive] bit NOT NULL DEFAULT 1,
                        [MoveOutDate] datetime2 NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        CONSTRAINT [PK_ResidentApartments] PRIMARY KEY ([ResidentApartmentId]),
                        CONSTRAINT [FK_ResidentApartments_Apartments_ApartmentId] FOREIGN KEY ([ApartmentId])
                            REFERENCES [Apartments] ([ApartmentId]) ON DELETE CASCADE,
                        CONSTRAINT [FK_ResidentApartments_Contracts_ContractId] FOREIGN KEY ([ContractId])
                            REFERENCES [Contracts] ([ContractId]) ON DELETE CASCADE,
                        CONSTRAINT [FK_ResidentApartments_Users_UserId] FOREIGN KEY ([UserId])
                            REFERENCES [Users] ([UserId]) ON DELETE CASCADE
                    );

                    CREATE INDEX [IX_ResidentApartments_ApartmentId] ON [ResidentApartments] ([ApartmentId]);
                    CREATE INDEX [IX_ResidentApartments_ContractId] ON [ResidentApartments] ([ContractId]);
                    CREATE UNIQUE INDEX [IX_ResidentApartments_UserId_ApartmentId_ContractId]
                        ON [ResidentApartments] ([UserId], [ApartmentId], [ContractId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ResidentApartments')
                    DROP TABLE [ResidentApartments];
            ");
        }
    }
}
