﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repositories;

namespace Repositories.Migrations
{
    [DbContext(typeof(DBContextBase))]
    [Migration("20200612085911_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Domain.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("AddressDetailName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("AddressName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("AttendanceGroupId");

                    b.Property<int>("Distance");

                    b.Property<double>("Latitude")
                        .HasColumnType("double(18,6)");

                    b.Property<double>("Longitude")
                        .HasColumnType("double(18,6)");

                    b.HasKey("AddressId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("Domain.AttendanceGroup", b =>
                {
                    b.Property<int>("AttendanceGroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("AttendanceGroupName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("AttendanceTypeEnum");

                    b.Property<bool>("AutomaticSchedule");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("FixedShift")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<bool>("IsClockAddress");

                    b.Property<bool>("IsWifi");

                    b.Property<int>("OvertimeRulesId");

                    b.Property<string>("ScheduleShift")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<TimeSpan>("TimeAcrossDays")
                        .HasColumnType("time");

                    b.HasKey("AttendanceGroupId");

                    b.ToTable("AttendanceGroup");
                });

            modelBuilder.Entity("Domain.ClockRecord", b =>
                {
                    b.Property<int>("ClockRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<double>("Address")
                        .HasMaxLength(50);

                    b.Property<DateTime>("AttendanceDate")
                        .HasColumnType("date");

                    b.Property<DateTime>("ClockTime")
                        .HasColumnType("datetime");

                    b.Property<int>("ClockType");

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<double>("Latitude")
                        .HasColumnType("double(18,6)");

                    b.Property<double>("Longitude")
                        .HasColumnType("double(18,6)");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Result");

                    b.Property<DateTime>("ShiftTime")
                        .HasColumnType("datetime");

                    b.HasKey("ClockRecordId");

                    b.ToTable("ClockRecord");
                });

            modelBuilder.Entity("Domain.ClockStatistics", b =>
                {
                    b.Property<int>("ClockStatisticsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int>("ActualAttendanceDay");

                    b.Property<TimeSpan>("ActualAttendanceTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("AllOvertime")
                        .HasColumnType("time");

                    b.Property<int>("AttendanceDay");

                    b.Property<TimeSpan>("AttendanceTime")
                        .HasColumnType("time");

                    b.Property<string>("ClockId")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<TimeSpan>("EarlyLeaveMinutes")
                        .HasColumnType("time");

                    b.Property<int>("EarlyLeaveTimes");

                    b.Property<TimeSpan>("GoOut")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("HolidayOvertime")
                        .HasColumnType("time");

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<TimeSpan>("LateMinutes")
                        .HasColumnType("time");

                    b.Property<int>("LateTime");

                    b.Property<TimeSpan>("Leave")
                        .HasColumnType("time");

                    b.Property<int>("NotClockInTimes");

                    b.Property<int>("NotClockOutTimes");

                    b.Property<TimeSpan>("RestOvertime")
                        .HasColumnType("time");

                    b.Property<DateTime>("SettingTime")
                        .HasColumnType("datetime");

                    b.Property<TimeSpan>("Travel")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("WorkTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("WorkingOvertime")
                        .HasColumnType("time");

                    b.HasKey("ClockStatisticsId");

                    b.ToTable("ClockStatistics");
                });

            modelBuilder.Entity("Domain.HolidayManagement", b =>
                {
                    b.Property<int>("HolidayManagementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("Balance")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime");

                    b.Property<string>("HolidayManagementName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime");

                    b.HasKey("HolidayManagementId");

                    b.ToTable("HolidayManagement");
                });

            modelBuilder.Entity("Domain.MonthStatistics", b =>
                {
                    b.Property<int>("MonthStatisticsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int>("ActualAttendanceDay");

                    b.Property<TimeSpan>("ActualAttendanceTime")
                        .HasColumnType("time");

                    b.Property<int>("AttendanceDay");

                    b.Property<TimeSpan>("AttendanceTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("EarlyLeaveMinutes")
                        .HasColumnType("time");

                    b.Property<int>("EarlyLeaveTimes");

                    b.Property<TimeSpan>("GoOut")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("Holiday")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("HolidayOvertime")
                        .HasColumnType("time");

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<TimeSpan>("LateMinutes")
                        .HasColumnType("time");

                    b.Property<int>("LateTime");

                    b.Property<int>("NotClockInTimes");

                    b.Property<int>("NotClockOutTimes");

                    b.Property<TimeSpan>("RestOvertime")
                        .HasColumnType("time");

                    b.Property<DateTime>("SettingTime")
                        .HasColumnType("datetime");

                    b.Property<TimeSpan>("Travel")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("WorkingOvertime")
                        .HasColumnType("time");

                    b.HasKey("MonthStatisticsId");

                    b.ToTable("MonthStatistics");
                });

            modelBuilder.Entity("Domain.PersonGroup", b =>
                {
                    b.Property<int>("PersonGroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int>("AttendanceGroupId");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.HasKey("PersonGroupId");

                    b.ToTable("PersonGroup");
                });

            modelBuilder.Entity("Domain.Scheduling", b =>
                {
                    b.Property<int>("SchedulingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("ShiftId");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime");

                    b.HasKey("SchedulingId");

                    b.ToTable("Scheduling");
                });

            modelBuilder.Entity("Domain.SetingDaytable", b =>
                {
                    b.Property<int>("SetingDaytableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int>("AttendanceGroupId");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("EarlyFlexible");

                    b.Property<int>("EarlyLeaveMinutes");

                    b.Property<string>("IdCard")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<bool>("IsExemption");

                    b.Property<bool>("IsFlexible");

                    b.Property<int>("LateFlexible");

                    b.Property<int>("LateMinutes");

                    b.Property<DateTime>("SettingTime")
                        .HasColumnType("datetime");

                    b.Property<string>("ShiftDetails")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<string>("ShiftName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("SetingDaytableId");

                    b.ToTable("SetingDaytable");
                });

            modelBuilder.Entity("Domain.Shift", b =>
                {
                    b.Property<int>("ShiftId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(36);

                    b.Property<int>("EarlyFlexible");

                    b.Property<int>("EarlyLeaveMinutes");

                    b.Property<bool>("IsExemption");

                    b.Property<bool>("IsFlexible");

                    b.Property<int>("LateFlexible");

                    b.Property<int>("LateMinutes");

                    b.Property<string>("ShiftName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ShiftId");

                    b.ToTable("Shift");
                });

            modelBuilder.Entity("Domain.ShiftDetail", b =>
                {
                    b.Property<int>("ShiftDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<TimeSpan>("DownEndClockTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("DownStartClockTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("EndRestTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("EndWorkTime")
                        .HasColumnType("time");

                    b.Property<bool>("IsEnableRest");

                    b.Property<bool>("IsEnableTime");

                    b.Property<int>("ShiftId");

                    b.Property<TimeSpan>("StartRestTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("StartWorkTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("UpEndClockTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("UpStartClockTime")
                        .HasColumnType("time");

                    b.HasKey("ShiftDetailId");

                    b.ToTable("ShiftDetail");
                });

            modelBuilder.Entity("Domain.Wifi", b =>
                {
                    b.Property<int>("WifiId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int>("AttendanceGroupId");

                    b.Property<string>("Mac")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("WifiName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("WifiId");

                    b.ToTable("Wifi");
                });
#pragma warning restore 612, 618
        }
    }
}
