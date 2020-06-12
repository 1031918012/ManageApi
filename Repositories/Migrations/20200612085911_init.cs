using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttendanceGroupId = table.Column<int>(nullable: false),
                    AddressName = table.Column<string>(maxLength: 50, nullable: false),
                    AddressDetailName = table.Column<string>(maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "double(18,6)", nullable: false),
                    Longitude = table.Column<double>(type: "double(18,6)", nullable: false),
                    Distance = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttendanceGroupName = table.Column<string>(maxLength: 50, nullable: false),
                    AttendanceTypeEnum = table.Column<int>(nullable: false),
                    FixedShift = table.Column<string>(type: "json", nullable: false),
                    AutomaticSchedule = table.Column<bool>(nullable: false),
                    TimeAcrossDays = table.Column<TimeSpan>(type: "time", nullable: false),
                    ScheduleShift = table.Column<string>(type: "json", nullable: false),
                    OvertimeRulesId = table.Column<int>(nullable: false),
                    IsClockAddress = table.Column<bool>(nullable: false),
                    IsWifi = table.Column<bool>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClockRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCard = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "date", nullable: false),
                    ShiftTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClockTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Address = table.Column<double>(maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "double(18,6)", nullable: false),
                    Longitude = table.Column<double>(type: "double(18,6)", nullable: false),
                    ClockType = table.Column<int>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(type: "json", nullable: false),
                    Remark = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClockStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCard = table.Column<string>(maxLength: 36, nullable: false),
                    SettingTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClockId = table.Column<string>(type: "json", nullable: false),
                    LateTime = table.Column<int>(nullable: false),
                    LateMinutes = table.Column<TimeSpan>(type: "time", nullable: false),
                    EarlyLeaveTimes = table.Column<int>(nullable: false),
                    EarlyLeaveMinutes = table.Column<TimeSpan>(type: "time", nullable: false),
                    WorkTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    NotClockInTimes = table.Column<int>(nullable: false),
                    NotClockOutTimes = table.Column<int>(nullable: false),
                    WorkingOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RestOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    HolidayOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AllOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Travel = table.Column<TimeSpan>(type: "time", nullable: false),
                    GoOut = table.Column<TimeSpan>(type: "time", nullable: false),
                    Leave = table.Column<TimeSpan>(type: "time", nullable: false),
                    AttendanceDay = table.Column<int>(nullable: false),
                    AttendanceTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ActualAttendanceDay = table.Column<int>(nullable: false),
                    ActualAttendanceTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayManagement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HolidayManagementName = table.Column<string>(maxLength: 50, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Balance = table.Column<string>(type: "json", nullable: false),
                    CustomerId = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayManagement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCard = table.Column<string>(maxLength: 36, nullable: false),
                    SettingTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LateTime = table.Column<int>(nullable: false),
                    LateMinutes = table.Column<TimeSpan>(type: "time", nullable: false),
                    EarlyLeaveTimes = table.Column<int>(nullable: false),
                    EarlyLeaveMinutes = table.Column<TimeSpan>(type: "time", nullable: false),
                    NotClockInTimes = table.Column<int>(nullable: false),
                    NotClockOutTimes = table.Column<int>(nullable: false),
                    WorkingOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RestOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    HolidayOvertime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Travel = table.Column<TimeSpan>(type: "time", nullable: false),
                    GoOut = table.Column<TimeSpan>(type: "time", nullable: false),
                    Holiday = table.Column<TimeSpan>(type: "time", nullable: false),
                    AttendanceDay = table.Column<int>(nullable: false),
                    AttendanceTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ActualAttendanceDay = table.Column<int>(nullable: false),
                    ActualAttendanceTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 25, nullable: false),
                    IdCard = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceGroupId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 36, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scheduling",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCard = table.Column<string>(maxLength: 36, nullable: false),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    ShiftId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scheduling", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SetingDaytable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCard = table.Column<string>(maxLength: 36, nullable: false),
                    SettingTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    AttendanceGroupId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 36, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 36, nullable: false),
                    ShiftName = table.Column<string>(maxLength: 50, nullable: false),
                    ShiftDetails = table.Column<string>(type: "json", nullable: false),
                    IsExemption = table.Column<bool>(nullable: false),
                    LateMinutes = table.Column<int>(nullable: false),
                    EarlyLeaveMinutes = table.Column<int>(nullable: false),
                    IsFlexible = table.Column<bool>(nullable: false),
                    EarlyFlexible = table.Column<int>(nullable: false),
                    LateFlexible = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetingDaytable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShiftName = table.Column<string>(maxLength: 50, nullable: false),
                    IsExemption = table.Column<bool>(nullable: false),
                    LateMinutes = table.Column<int>(nullable: false),
                    EarlyLeaveMinutes = table.Column<int>(nullable: false),
                    IsFlexible = table.Column<bool>(nullable: false),
                    EarlyFlexible = table.Column<int>(nullable: false),
                    LateFlexible = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShiftId = table.Column<int>(nullable: false),
                    StartWorkTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndWorkTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsEnableRest = table.Column<bool>(nullable: false),
                    StartRestTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndRestTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsEnableTime = table.Column<bool>(nullable: false),
                    UpStartClockTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpEndClockTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DownStartClockTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DownEndClockTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wifi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttendanceGroupId = table.Column<int>(nullable: false),
                    WifiName = table.Column<string>(maxLength: 50, nullable: false),
                    Mac = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wifi", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "AttendanceGroup");

            migrationBuilder.DropTable(
                name: "ClockRecord");

            migrationBuilder.DropTable(
                name: "ClockStatistics");

            migrationBuilder.DropTable(
                name: "HolidayManagement");

            migrationBuilder.DropTable(
                name: "MonthStatistics");

            migrationBuilder.DropTable(
                name: "PersonGroup");

            migrationBuilder.DropTable(
                name: "Scheduling");

            migrationBuilder.DropTable(
                name: "SetingDaytable");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "ShiftDetail");

            migrationBuilder.DropTable(
                name: "Wifi");
        }
    }
}
