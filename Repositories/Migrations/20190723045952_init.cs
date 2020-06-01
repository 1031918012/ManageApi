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
                name: "AttendanceGroup",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceRuleID = table.Column<string>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    ShiftType = table.Column<int>(nullable: false),
                    ClockInWay = table.Column<int>(nullable: false),
                    SiteAttendance = table.Column<string>(type: "json", nullable: false),
                    IsDynamicRowHugh = table.Column<bool>(nullable: false),
                    CompanyID = table.Column<string>(maxLength: 36, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 20, nullable: false),
                    Creator = table.Column<string>(maxLength: 20, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Range = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceGroup", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceItemCatagory",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemCatagoryName = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(maxLength: 50, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceItemCatagory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceMonthlyRecord",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    IDCard = table.Column<string>(maxLength: 18, nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EmployeeNo = table.Column<string>(maxLength: 50, nullable: false),
                    Department = table.Column<string>(maxLength: 50, nullable: false),
                    Position = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyID = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 100, nullable: false),
                    AttendanceProjectsJson = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceMonthlyRecord", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    IDCard = table.Column<string>(maxLength: 18, nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    EmployeeNo = table.Column<string>(maxLength: 50, nullable: false),
                    Department = table.Column<string>(maxLength: 50, nullable: false),
                    Position = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyID = table.Column<string>(maxLength: 36, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 50, nullable: false),
                    AttendanceGroupID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceGroupName = table.Column<string>(maxLength: 50, nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Shift = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ClockIn1 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockInResult1 = table.Column<int>(nullable: false),
                    ClockInAddress1 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockOut1 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockOutResult1 = table.Column<int>(nullable: false),
                    ClockOutAddress1 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockIn2 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockInResult2 = table.Column<int>(nullable: false),
                    ClockInAddress2 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockOut2 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockOutResult2 = table.Column<int>(nullable: false),
                    ClockOutAddress2 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockIn3 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockInResult3 = table.Column<int>(nullable: false),
                    ClockInAddress3 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockOut3 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockOutResult3 = table.Column<int>(nullable: false),
                    ClockOutAddress3 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockIn4 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockInResult4 = table.Column<int>(nullable: false),
                    ClockInAddress4 = table.Column<string>(maxLength: 50, nullable: false),
                    ClockOut4 = table.Column<DateTime>(type: "datetime", nullable: true),
                    ClockOutResult4 = table.Column<int>(nullable: false),
                    ClockOutAddress4 = table.Column<string>(maxLength: 50, nullable: false),
                    LateTimes = table.Column<int>(nullable: false),
                    LateMinutes = table.Column<int>(nullable: false),
                    EarlyLeaveTimes = table.Column<int>(nullable: false),
                    EarlyLeaveMinutes = table.Column<int>(nullable: false),
                    NotClockInTimes = table.Column<int>(nullable: false),
                    NotClockOutTimes = table.Column<int>(nullable: false),
                    BusinessTripDuration = table.Column<int>(nullable: false),
                    OutsideDuration = table.Column<int>(nullable: false),
                    LeaveDuration = table.Column<int>(nullable: false),
                    WorkingDayOvertime = table.Column<int>(nullable: false),
                    RestDayOvertime = table.Column<int>(nullable: false),
                    HoliDayOvertime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRule",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    RuleName = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyID = table.Column<string>(maxLength: 36, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 20, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false),
                    Creator = table.Column<string>(maxLength: 20, nullable: false),
                    Remark = table.Column<string>(maxLength: 200, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LateRule = table.Column<int>(nullable: false),
                    EarlyLeaveRule = table.Column<int>(nullable: false),
                    NotClockRule = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRule", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRuleDetail",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceRuleID = table.Column<string>(maxLength: 36, nullable: false),
                    RuleType = table.Column<int>(maxLength: 10, nullable: false),
                    MinTime = table.Column<int>(nullable: false),
                    MinJudge = table.Column<int>(nullable: false),
                    MaxJudge = table.Column<int>(nullable: false),
                    MaxTime = table.Column<int>(nullable: false),
                    CallRuleType = table.Column<int>(maxLength: 10, nullable: false),
                    Time = table.Column<int>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    Sort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRuleDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ClockInAddress",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    SiteName = table.Column<string>(maxLength: 100, nullable: false),
                    ClockName = table.Column<string>(maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "double(18,6)", nullable: false),
                    Longitude = table.Column<double>(type: "double(18,6)", nullable: false),
                    LatitudeBD = table.Column<double>(type: "double(18,6)", nullable: false),
                    LongitudeBD = table.Column<double>(type: "double(18,6)", nullable: false),
                    Distance = table.Column<int>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    CompanyID = table.Column<string>(maxLength: 36, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 20, nullable: false),
                    Creator = table.Column<string>(maxLength: 20, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockInAddress", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ClockRecord",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IDCard = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    EmployeeNo = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    CompanyID = table.Column<string>(nullable: false),
                    CompanyName = table.Column<string>(nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClockTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClockType = table.Column<int>(nullable: false),
                    ClockResult = table.Column<int>(nullable: false),
                    ClockWay = table.Column<int>(nullable: false),
                    IsInRange = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    AbnormalReason = table.Column<string>(nullable: true),
                    ClockImage1 = table.Column<string>(nullable: true),
                    ClockImage2 = table.Column<string>(nullable: true),
                    ClockDevice = table.Column<string>(nullable: true),
                    ShiftTimeID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockRecord", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseSet",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    CompanyID = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 50, nullable: false),
                    SortNumber = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(maxLength: 50, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemName = table.Column<string>(maxLength: 50, nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseSet", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GroupPersonnel",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceGroupID = table.Column<string>(maxLength: 36, nullable: false),
                    IDCard = table.Column<string>(maxLength: 18, nullable: false),
                    CompanyID = table.Column<string>(maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPersonnel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    HolidayName = table.Column<string>(maxLength: 50, nullable: false),
                    HolidayYear = table.Column<int>(nullable: false),
                    HolidayNumber = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    StartHolidayTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndHolidayTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Department = table.Column<string>(maxLength: 100, nullable: false),
                    Position = table.Column<string>(maxLength: 50, nullable: false),
                    IDType = table.Column<string>(maxLength: 50, nullable: false),
                    IDCard = table.Column<string>(maxLength: 18, nullable: false),
                    PhoneCode = table.Column<string>(maxLength: 15, nullable: false),
                    JobNumber = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyID = table.Column<string>(maxLength: 36, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 50, nullable: false),
                    IsBindWechat = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ShiftManagement",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    ShiftName = table.Column<string>(maxLength: 50, nullable: false),
                    AttendanceTime = table.Column<string>(maxLength: 100, nullable: false),
                    WorkHours = table.Column<decimal>(type: "decimal(18,1)", nullable: false),
                    ShiftRemark = table.Column<string>(maxLength: 200, nullable: false),
                    ClockRule = table.Column<int>(nullable: false),
                    CompanyID = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(maxLength: 50, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsExemption = table.Column<bool>(type: "bit", nullable: false),
                    LateMinutes = table.Column<int>(nullable: false),
                    EarlyLeaveMinutes = table.Column<int>(nullable: false),
                    IsFlexible = table.Column<bool>(type: "bit", nullable: false),
                    FlexibleMinutes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftManagement", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkPaidLeave",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    PaidLeaveTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Type = table.Column<int>(nullable: false),
                    HolidayID = table.Column<string>(maxLength: 36, nullable: false),
                    HolidayName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPaidLeave", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WeekDaysSetting",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceGroupID = table.Column<string>(maxLength: 36, nullable: false),
                    Week = table.Column<int>(nullable: false),
                    ShiftID = table.Column<string>(maxLength: 36, nullable: false),
                    IsHolidayWork = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDaysSetting", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WeekDaysSetting_AttendanceGroup_AttendanceGroupID",
                        column: x => x.AttendanceGroupID,
                        principalTable: "AttendanceGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceItem",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemName = table.Column<string>(maxLength: 50, nullable: false),
                    AttendanceItemCatagoryID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemCatagoryName = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(maxLength: 50, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttendanceItem_AttendanceItemCatagory_AttendanceItemCatagory~",
                        column: x => x.AttendanceItemCatagoryID,
                        principalTable: "AttendanceItemCatagory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnterpriseSetUnit",
                columns: table => new
                {
                    EnterpriseSetUnitID = table.Column<string>(nullable: false),
                    SortNumber = table.Column<int>(nullable: false),
                    EnterpriseSetID = table.Column<string>(nullable: true),
                    AttendanceItemUnitID = table.Column<string>(nullable: true),
                    AttendanceItemUnitName = table.Column<string>(nullable: true),
                    IsSelect = table.Column<bool>(nullable: false),
                    CompanyID = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterpriseSetUnit", x => x.EnterpriseSetUnitID);
                    table.ForeignKey(
                        name: "FK_EnterpriseSetUnit_EnterpriseSet_EnterpriseSetID",
                        column: x => x.EnterpriseSetID,
                        principalTable: "EnterpriseSet",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTimeManagement",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    ShiftID = table.Column<string>(maxLength: 36, nullable: false),
                    ShiftName = table.Column<string>(maxLength: 50, nullable: false),
                    ShiftTimeNumber = table.Column<int>(nullable: false),
                    StartWorkTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndWorkTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    StartRestTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndRestTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpStartClockTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpEndClockTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    DownStartClockTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    DownEndClockTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTimeManagement", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShiftTimeManagement_ShiftManagement_ShiftID",
                        column: x => x.ShiftID,
                        principalTable: "ShiftManagement",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceItemUnit",
                columns: table => new
                {
                    ID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemUnitName = table.Column<string>(maxLength: 10, nullable: false),
                    AttendanceItemID = table.Column<string>(maxLength: 36, nullable: false),
                    AttendanceItemName = table.Column<string>(maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(maxLength: 50, nullable: false),
                    CreatorID = table.Column<string>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceItemUnit", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttendanceItemUnit_AttendanceItem_AttendanceItemID",
                        column: x => x.AttendanceItemID,
                        principalTable: "AttendanceItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceItem_AttendanceItemCatagoryID",
                table: "AttendanceItem",
                column: "AttendanceItemCatagoryID");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceItemUnit_AttendanceItemID",
                table: "AttendanceItemUnit",
                column: "AttendanceItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ClockRecord_IDCard",
                table: "ClockRecord",
                column: "IDCard");

            migrationBuilder.CreateIndex(
                name: "IX_EnterpriseSetUnit_EnterpriseSetID",
                table: "EnterpriseSetUnit",
                column: "EnterpriseSetID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IDCard_ID",
                table: "Person",
                columns: new[] { "IDCard", "ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTimeManagement_ShiftID",
                table: "ShiftTimeManagement",
                column: "ShiftID");

            migrationBuilder.CreateIndex(
                name: "IX_WeekDaysSetting_AttendanceGroupID",
                table: "WeekDaysSetting",
                column: "AttendanceGroupID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceItemUnit");

            migrationBuilder.DropTable(
                name: "AttendanceMonthlyRecord");

            migrationBuilder.DropTable(
                name: "AttendanceRecord");

            migrationBuilder.DropTable(
                name: "AttendanceRule");

            migrationBuilder.DropTable(
                name: "AttendanceRuleDetail");

            migrationBuilder.DropTable(
                name: "ClockInAddress");

            migrationBuilder.DropTable(
                name: "ClockRecord");

            migrationBuilder.DropTable(
                name: "EnterpriseSetUnit");

            migrationBuilder.DropTable(
                name: "GroupPersonnel");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "ShiftTimeManagement");

            migrationBuilder.DropTable(
                name: "WeekDaysSetting");

            migrationBuilder.DropTable(
                name: "WorkPaidLeave");

            migrationBuilder.DropTable(
                name: "AttendanceItem");

            migrationBuilder.DropTable(
                name: "EnterpriseSet");

            migrationBuilder.DropTable(
                name: "ShiftManagement");

            migrationBuilder.DropTable(
                name: "AttendanceGroup");

            migrationBuilder.DropTable(
                name: "AttendanceItemCatagory");
        }
    }
}
