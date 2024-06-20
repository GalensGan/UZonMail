﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UZonMailService.Models.SqlLite;

#nullable disable

namespace UZonMailService.Migrations
{
    [DbContext(typeof(SqlContext))]
    partial class SqlContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("EmailTemplateSendingGroup", b =>
                {
                    b.Property<int>("SendingGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TemplatesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SendingGroupId", "TemplatesId");

                    b.HasIndex("TemplatesId");

                    b.ToTable("EmailTemplateSendingGroup");
                });

            modelBuilder.Entity("FileUsageSendingGroup", b =>
                {
                    b.Property<int>("AttachmentsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SendingGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AttachmentsId", "SendingGroupId");

                    b.HasIndex("SendingGroupId");

                    b.ToTable("FileUsageSendingGroup");
                });

            modelBuilder.Entity("FileUsageSendingItem", b =>
                {
                    b.Property<int>("AttachmentsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SendingItemId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AttachmentsId", "SendingItemId");

                    b.HasIndex("SendingItemId");

                    b.ToTable("FileUsageSendingItem");
                });

            modelBuilder.Entity("OutboxSendingGroup", b =>
                {
                    b.Property<int>("OutboxesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SendingGroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("OutboxesId", "SendingGroupId");

                    b.HasIndex("SendingGroupId");

                    b.ToTable("OutboxSendingGroup");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.EmailSending.SendingGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BccBoxes")
                        .HasColumnType("TEXT");

                    b.Property<string>("Body")
                        .HasColumnType("TEXT");

                    b.Property<string>("CcBoxes")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Data")
                        .HasColumnType("TEXT");

                    b.Property<string>("Inboxes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDistributed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastMessage")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ScheduleDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SendEndDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SendStartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("SendingType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SentCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Subjects")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SuccessCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("SendingGroups");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.EmailSending.SendingItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BCC")
                        .HasColumnType("TEXT");

                    b.Property<string>("CC")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("EmailTemplateId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FromEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("Inboxes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSendingBatch")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OutBoxId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProxyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReceiptId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SendDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("SendResult")
                        .HasColumnType("TEXT");

                    b.Property<int>("SendingGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Subject")
                        .HasColumnType("TEXT");

                    b.Property<int>("TriedCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SendingGroupId");

                    b.ToTable("SendingItems");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Emails.EmailGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EmailGroups");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Emails.Inbox", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoxType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("EmailGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LinkCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EmailGroupId");

                    b.ToTable("Inboxes");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Inbox");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Files.FileBucket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BucketName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RootDir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FileBuckets");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Files.FileObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("FileBucketId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModifyDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("LinkCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FileBucketId");

                    b.ToTable("FileObjects");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Files.FileUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FileObjectId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OwnerUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UniqueName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FileObjectId");

                    b.HasIndex("OwnerUserId");

                    b.ToTable("FileUsages");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.PermissionCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("PermissionCodes");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.RolePermissionCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PermissionCodeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("RolePermissionCodes");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Settings.SystemSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("InitializedQuartz")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SystemSettings");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Settings.UserProxy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsShared")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MatchRegex")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Proxy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("UserProxies");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Settings.UserSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxOutboxCooldownSecond")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxSendCountPerEmailDay")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxSendingBatchSize")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinOutboxCooldownSecond")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Templates.EmailTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("EmailTemplates");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.UserInfos.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Avatar")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ForbiddenToLogin")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSuperAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSystemUser")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("RoleId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Emails.Outbox", b =>
                {
                    b.HasBaseType("UZonMailService.Models.SqlLite.Emails.Inbox");

                    b.Property<bool>("EnableSSL")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxSendCountPerDay")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ProxyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SmtpHost")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SmtpPort")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("Outbox");
                });

            modelBuilder.Entity("EmailTemplateSendingGroup", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.EmailSending.SendingGroup", null)
                        .WithMany()
                        .HasForeignKey("SendingGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UZonMailService.Models.SqlLite.Templates.EmailTemplate", null)
                        .WithMany()
                        .HasForeignKey("TemplatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FileUsageSendingGroup", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Files.FileUsage", null)
                        .WithMany()
                        .HasForeignKey("AttachmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UZonMailService.Models.SqlLite.EmailSending.SendingGroup", null)
                        .WithMany()
                        .HasForeignKey("SendingGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FileUsageSendingItem", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Files.FileUsage", null)
                        .WithMany()
                        .HasForeignKey("AttachmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UZonMailService.Models.SqlLite.EmailSending.SendingItem", null)
                        .WithMany()
                        .HasForeignKey("SendingItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OutboxSendingGroup", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Emails.Outbox", null)
                        .WithMany()
                        .HasForeignKey("OutboxesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UZonMailService.Models.SqlLite.EmailSending.SendingGroup", null)
                        .WithMany()
                        .HasForeignKey("SendingGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.EmailSending.SendingItem", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.EmailSending.SendingGroup", "SendingGroup")
                        .WithMany()
                        .HasForeignKey("SendingGroupId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("SendingGroup");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Emails.EmailGroup", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.UserInfos.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Emails.Inbox", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Emails.EmailGroup", null)
                        .WithMany("Inboxes")
                        .HasForeignKey("EmailGroupId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Files.FileObject", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Files.FileBucket", "FileBucket")
                        .WithMany()
                        .HasForeignKey("FileBucketId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("FileBucket");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Files.FileUsage", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Files.FileObject", "FileObject")
                        .WithMany()
                        .HasForeignKey("FileObjectId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("UZonMailService.Models.SqlLite.UserInfos.User", "OwnerUser")
                        .WithMany()
                        .HasForeignKey("OwnerUserId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("FileObject");

                    b.Navigation("OwnerUser");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.PermissionCode", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Permission.Role", null)
                        .WithMany("PermissionCodes")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientNoAction);
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.UserRole", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Permission.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("UZonMailService.Models.SqlLite.UserInfos.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.UserInfos.User", b =>
                {
                    b.HasOne("UZonMailService.Models.SqlLite.Permission.Role", null)
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientNoAction);
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Emails.EmailGroup", b =>
                {
                    b.Navigation("Inboxes");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.Permission.Role", b =>
                {
                    b.Navigation("PermissionCodes");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("UZonMailService.Models.SqlLite.UserInfos.User", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}