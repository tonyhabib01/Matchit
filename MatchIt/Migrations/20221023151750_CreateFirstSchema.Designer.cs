﻿// <auto-generated />
using System;
using MatchIt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MatchIt.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20221023151750_CreateFirstSchema")]
    partial class CreateFirstSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CourseStudent", b =>
                {
                    b.Property<int>("CoursesId")
                        .HasColumnType("int");

                    b.Property<int>("StudentsId")
                        .HasColumnType("int");

                    b.HasKey("CoursesId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("CourseStudent");
                });

            modelBuilder.Entity("MatchIt.Models.Availability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Day")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("From")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("To")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("Availabilities");
                });

            modelBuilder.Entity("MatchIt.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("MatchIt.Models.MatchingStudents", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<int?>("TuteeId")
                        .HasColumnType("int");

                    b.Property<int?>("TutorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("TuteeId");

                    b.HasIndex("TutorId");

                    b.ToTable("MatchingStudents");
                });

            modelBuilder.Entity("MatchIt.Models.Semester", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Semesters");
                });

            modelBuilder.Entity("MatchIt.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SemesterId")
                        .HasColumnType("int");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StudentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SemesterId");

                    b.ToTable("Students");

                    b.HasDiscriminator<string>("StudentType").HasValue("student");
                });

            modelBuilder.Entity("MatchIt.Models.Tutee", b =>
                {
                    b.HasBaseType("MatchIt.Models.Student");

                    b.HasDiscriminator().HasValue("tutee");
                });

            modelBuilder.Entity("MatchIt.Models.Tutor", b =>
                {
                    b.HasBaseType("MatchIt.Models.Student");

                    b.Property<bool>("IsVolunteer")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("tutor");
                });

            modelBuilder.Entity("CourseStudent", b =>
                {
                    b.HasOne("MatchIt.Models.Course", null)
                        .WithMany()
                        .HasForeignKey("CoursesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MatchIt.Models.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MatchIt.Models.Availability", b =>
                {
                    b.HasOne("MatchIt.Models.Student", "Student")
                        .WithMany("Availabilities")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("MatchIt.Models.MatchingStudents", b =>
                {
                    b.HasOne("MatchIt.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");

                    b.HasOne("MatchIt.Models.Tutee", "Tutee")
                        .WithMany("MatchingTutors")
                        .HasForeignKey("TuteeId");

                    b.HasOne("MatchIt.Models.Tutor", "Tutor")
                        .WithMany("MatchingTutees")
                        .HasForeignKey("TutorId");

                    b.Navigation("Course");

                    b.Navigation("Tutee");

                    b.Navigation("Tutor");
                });

            modelBuilder.Entity("MatchIt.Models.Student", b =>
                {
                    b.HasOne("MatchIt.Models.Semester", "Semester")
                        .WithMany("Students")
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Semester");
                });

            modelBuilder.Entity("MatchIt.Models.Semester", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("MatchIt.Models.Student", b =>
                {
                    b.Navigation("Availabilities");
                });

            modelBuilder.Entity("MatchIt.Models.Tutee", b =>
                {
                    b.Navigation("MatchingTutors");
                });

            modelBuilder.Entity("MatchIt.Models.Tutor", b =>
                {
                    b.Navigation("MatchingTutees");
                });
#pragma warning restore 612, 618
        }
    }
}
