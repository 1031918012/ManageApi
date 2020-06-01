﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repositories;

namespace Repositories.Migrations
{
    [DbContext(typeof(DBContextBase))]
    partial class DBContextBaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Domain.AttendanceGroup", b =>
                {
                    b.Property<string>("AttendanceGroupID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceRuleID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("ClockInWay");

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<bool>("IsDynamicRowHugh");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Range")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("ShiftType");

                    b.Property<string>("SiteAttendance")
                        .IsRequired()
                        .HasColumnType("json");

                    b.HasKey("AttendanceGroupID");

                    b.ToTable("AttendanceGroup");
                });

            modelBuilder.Entity("Domain.AttendanceItem", b =>
                {
                    b.Property<string>("AttendanceItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemCatagoryID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemCatagoryName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("AttendanceItemName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.HasKey("AttendanceItemID");

                    b.HasIndex("AttendanceItemCatagoryID");

                    b.ToTable("AttendanceItem");
                });

            modelBuilder.Entity("Domain.AttendanceItemCatagory", b =>
                {
                    b.Property<string>("AttendanceItemCatagoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemCatagoryName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.HasKey("AttendanceItemCatagoryID");

                    b.ToTable("AttendanceItemCatagory");
                });

            modelBuilder.Entity("Domain.AttendanceItemUnit", b =>
                {
                    b.Property<string>("AttendanceItemUnitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("AttendanceItemUnitName")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.HasKey("AttendanceItemUnitID");

                    b.HasIndex("AttendanceItemID");

                    b.ToTable("AttendanceItemUnit");
                });

            modelBuilder.Entity("Domain.AttendanceMonthlyRecord", b =>
                {
                    b.Property<string>("AttendanceMonthlyRecordID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<DateTime>("AttendanceDate")
                        .HasColumnType("datetime");

                    b.Property<string>("AttendanceProjectsJson")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("EmployeeNo")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("IDCard")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("AttendanceMonthlyRecordID");

                    b.ToTable("AttendanceMonthlyRecord");
                });

            modelBuilder.Entity("Domain.AttendanceRecord", b =>
                {
                    b.Property<string>("AttendanceRecordID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<DateTime>("AttendanceDate")
                        .HasColumnType("datetime");

                    b.Property<string>("AttendanceGroupID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceGroupName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("BusinessTripDuration");

                    b.Property<DateTime?>("ClockIn1")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ClockIn2")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ClockIn3")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ClockIn4")
                        .HasColumnType("datetime");

                    b.Property<string>("ClockInAddress1")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClockInAddress2")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClockInAddress3")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClockInAddress4")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("ClockInResult1");

                    b.Property<int>("ClockInResult2");

                    b.Property<int>("ClockInResult3");

                    b.Property<int>("ClockInResult4");

                    b.Property<DateTime?>("ClockOut1")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ClockOut2")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ClockOut3")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ClockOut4")
                        .HasColumnType("datetime");

                    b.Property<string>("ClockOutAddress1")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClockOutAddress2")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClockOutAddress3")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClockOutAddress4")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("ClockOutResult1");

                    b.Property<int>("ClockOutResult2");

                    b.Property<int>("ClockOutResult3");

                    b.Property<int>("ClockOutResult4");

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("EarlyLeaveMinutes");

                    b.Property<int>("EarlyLeaveTimes");

                    b.Property<string>("EmployeeNo")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("HoliDayOvertime");

                    b.Property<string>("IDCard")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<int>("LateMinutes");

                    b.Property<int>("LateTimes");

                    b.Property<int>("LeaveDuration");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int>("NotClockInTimes");

                    b.Property<int>("NotClockOutTimes");

                    b.Property<int>("OutsideDuration");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("RestDayOvertime");

                    b.Property<string>("Shift")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Status");

                    b.Property<int>("WorkingDayOvertime");

                    b.HasKey("AttendanceRecordID");

                    b.ToTable("AttendanceRecord");
                });

            modelBuilder.Entity("Domain.AttendanceRule", b =>
                {
                    b.Property<string>("AttendanceRuleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("EarlyLeaveRule");

                    b.Property<int>("LateRule");

                    b.Property<int>("NotClockRule");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("RuleName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("AttendanceRuleID");

                    b.ToTable("AttendanceRule");
                });

            modelBuilder.Entity("Domain.AttendanceRuleDetail", b =>
                {
                    b.Property<string>("AttendanceRuleDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceRuleID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("CallRuleType")
                        .HasMaxLength(10);

                    b.Property<int>("MaxJudge");

                    b.Property<int>("MaxTime");

                    b.Property<int>("MinJudge");

                    b.Property<int>("MinTime");

                    b.Property<int>("RuleType")
                        .HasMaxLength(10);

                    b.Property<int>("Sort");

                    b.Property<int>("Time");

                    b.Property<int>("Unit");

                    b.HasKey("AttendanceRuleDetailID");

                    b.ToTable("AttendanceRuleDetail");
                });

            modelBuilder.Entity("Domain.ClockInAddress", b =>
                {
                    b.Property<string>("ClockInAddressID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("ClockName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("Distance");

                    b.Property<bool>("IsDelete");

                    b.Property<double>("Latitude")
                        .HasColumnType("double(18,6)");

                    b.Property<double>("LatitudeBD")
                        .HasColumnType("double(18,6)");

                    b.Property<double>("Longitude")
                        .HasColumnType("double(18,6)");

                    b.Property<double>("LongitudeBD")
                        .HasColumnType("double(18,6)");

                    b.Property<string>("SiteName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("ClockInAddressID");

                    b.ToTable("ClockInAddress");
                });

            modelBuilder.Entity("Domain.ClockRecord", b =>
                {
                    b.Property<int>("ClockRecordID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID");

                    b.Property<string>("AbnormalReason");

                    b.Property<DateTime>("AttendanceDate")
                        .HasColumnType("datetime");

                    b.Property<string>("ClockDevice");

                    b.Property<string>("ClockImage1");

                    b.Property<string>("ClockImage2");

                    b.Property<int>("ClockResult");

                    b.Property<DateTime>("ClockTime")
                        .HasColumnType("datetime");

                    b.Property<int>("ClockType");

                    b.Property<int>("ClockWay");

                    b.Property<string>("CompanyID")
                        .IsRequired();

                    b.Property<string>("CompanyName")
                        .IsRequired();

                    b.Property<string>("Department");

                    b.Property<string>("EmployeeNo");

                    b.Property<string>("IDCard")
                        .IsRequired();

                    b.Property<bool>("IsInRange")
                        .HasColumnType("bit");

                    b.Property<string>("Location");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Position");

                    b.Property<string>("Remark");

                    b.Property<string>("ShiftTimeID");

                    b.HasKey("ClockRecordID");

                    b.HasIndex("IDCard");

                    b.ToTable("ClockRecord");
                });

            modelBuilder.Entity("Domain.EnterpriseSet", b =>
                {
                    b.Property<string>("EnterpriseSetID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceItemName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<bool>("IsEnable")
                        .HasColumnType("bit");

                    b.Property<int>("SortNumber");

                    b.HasKey("EnterpriseSetID");

                    b.ToTable("EnterpriseSet");
                });

            modelBuilder.Entity("Domain.EnterpriseSetUnit", b =>
                {
                    b.Property<string>("EnterpriseSetUnitID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AttendanceItemUnitID");

                    b.Property<string>("AttendanceItemUnitName");

                    b.Property<string>("CompanyID");

                    b.Property<string>("CompanyName");

                    b.Property<string>("EnterpriseSetID");

                    b.Property<bool>("IsSelect");

                    b.Property<int>("SortNumber");

                    b.HasKey("EnterpriseSetUnitID");

                    b.HasIndex("EnterpriseSetID");

                    b.ToTable("EnterpriseSetUnit");
                });

            modelBuilder.Entity("Domain.GroupPersonnel", b =>
                {
                    b.Property<string>("GroupPersonnelID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceGroupID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("IDCard")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.HasKey("GroupPersonnelID");

                    b.ToTable("GroupPersonnel");
                });

            modelBuilder.Entity("Domain.Holiday", b =>
                {
                    b.Property<string>("HolidayID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("EndHolidayTime")
                        .HasColumnType("datetime");

                    b.Property<string>("HolidayName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("HolidayNumber");

                    b.Property<int>("HolidayYear");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartHolidayTime")
                        .HasColumnType("datetime");

                    b.HasKey("HolidayID");

                    b.ToTable("Holiday");
                });

            modelBuilder.Entity("Domain.Person", b =>
                {
                    b.Property<string>("PersonID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("IDCard")
                        .IsRequired()
                        .HasMaxLength(18);

                    b.Property<string>("IDType")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("IsBindWechat");

                    b.Property<string>("JobNumber")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PhoneCode")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("PersonID");

                    b.HasIndex("IDCard", "PersonID")
                        .IsUnique();

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Domain.ShiftManagement", b =>
                {
                    b.Property<string>("ShiftID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceTime")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("ClockRule");

                    b.Property<string>("CompanyID")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CreatorID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("EarlyLeaveMinutes");

                    b.Property<int>("FlexibleMinutes");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<bool>("IsExemption")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFlexible")
                        .HasColumnType("bit");

                    b.Property<int>("LateMinutes");

                    b.Property<string>("ShiftName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ShiftRemark")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<decimal>("WorkHours")
                        .HasColumnType("decimal(18,1)");

                    b.HasKey("ShiftID");

                    b.ToTable("ShiftManagement");
                });

            modelBuilder.Entity("Domain.ShiftTimeManagement", b =>
                {
                    b.Property<string>("ShiftTimeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<DateTime>("DownEndClockTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DownStartClockTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("EndRestTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("EndWorkTime")
                        .HasColumnType("datetime");

                    b.Property<string>("ShiftID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("ShiftName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("ShiftTimeNumber");

                    b.Property<DateTime?>("StartRestTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("StartWorkTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("UpEndClockTime")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("UpStartClockTime")
                        .HasColumnType("datetime");

                    b.HasKey("ShiftTimeID");

                    b.HasIndex("ShiftID");

                    b.ToTable("ShiftTimeManagement");
                });

            modelBuilder.Entity("Domain.WeekDaysSetting", b =>
                {
                    b.Property<string>("WeekDaysSettingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("AttendanceGroupID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<bool>("IsHolidayWork");

                    b.Property<string>("ShiftID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("Week");

                    b.HasKey("WeekDaysSettingID");

                    b.HasIndex("AttendanceGroupID");

                    b.ToTable("WeekDaysSetting");
                });

            modelBuilder.Entity("Domain.WorkPaidLeave", b =>
                {
                    b.Property<string>("WorkPaidLeaveID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasMaxLength(36);

                    b.Property<string>("HolidayID")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("HolidayName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("PaidLeaveTime")
                        .HasColumnType("datetime");

                    b.Property<int>("Type");

                    b.HasKey("WorkPaidLeaveID");

                    b.ToTable("WorkPaidLeave");
                });

            modelBuilder.Entity("Domain.AttendanceItem", b =>
                {
                    b.HasOne("Domain.AttendanceItemCatagory")
                        .WithMany("AttendanceItemList")
                        .HasForeignKey("AttendanceItemCatagoryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.AttendanceItemUnit", b =>
                {
                    b.HasOne("Domain.AttendanceItem")
                        .WithMany("AttendanceItemUnitList")
                        .HasForeignKey("AttendanceItemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.EnterpriseSetUnit", b =>
                {
                    b.HasOne("Domain.EnterpriseSet")
                        .WithMany("EnterpriseSetUnitList")
                        .HasForeignKey("EnterpriseSetID");
                });

            modelBuilder.Entity("Domain.ShiftTimeManagement", b =>
                {
                    b.HasOne("Domain.ShiftManagement")
                        .WithMany("ShiftTimeManagementList")
                        .HasForeignKey("ShiftID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.WeekDaysSetting", b =>
                {
                    b.HasOne("Domain.AttendanceGroup")
                        .WithMany("WeekDaysSettings")
                        .HasForeignKey("AttendanceGroupID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
