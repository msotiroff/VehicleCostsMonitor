namespace VehicleCostsMonitor.Data.Migrations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;
    using System;

    [DbContext(typeof(JustMonitorDbContext))]
    [Migration("20180808212816_CostsSumRemovedFromVehicle")]
    partial class CostsSumRemovedFromVehicle
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.CostEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CostEntryTypeId");

                    b.Property<int>("CurrencyId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Note");

                    b.Property<int?>("Odometer");

                    b.Property<decimal>("Price");

                    b.Property<int>("VehicleId");

                    b.HasKey("Id");

                    b.HasIndex("CostEntryTypeId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("VehicleId");

                    b.ToTable("CostEntries");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.CostEntryType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("CostEntryTypes");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<string>("DisplayName");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.ExtraFuelConsumer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ExtraFuelConsumers");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Average");

                    b.Property<int>("CurrencyId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("FuelEntryTypeId");

                    b.Property<double>("FuelQuantity");

                    b.Property<int>("FuelTypeId");

                    b.Property<string>("Note");

                    b.Property<int>("Odometer");

                    b.Property<decimal>("Price");

                    b.Property<int>("TripOdometer");

                    b.Property<int>("VehicleId");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("FuelEntryTypeId");

                    b.HasIndex("FuelTypeId");

                    b.HasIndex("VehicleId");

                    b.ToTable("FuelEntries");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntryExtraFuelConsumer", b =>
                {
                    b.Property<int>("FuelEntryId");

                    b.Property<int>("ExtraFuelConsumerId");

                    b.HasKey("FuelEntryId", "ExtraFuelConsumerId");

                    b.HasIndex("ExtraFuelConsumerId");

                    b.ToTable("FuelEntryExtraFuelConsumers");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntryRouteType", b =>
                {
                    b.Property<int>("FuelEntryId");

                    b.Property<int>("RouteTypeId");

                    b.HasKey("FuelEntryId", "RouteTypeId");

                    b.HasIndex("RouteTypeId");

                    b.ToTable("FuelEntryRouteTypes");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntryType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("FuelEntryTypes");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("FuelTypes");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.GearingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("GearingTypes");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Manufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Manufacturers");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Model", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ManufacturerId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ManufacturerId");

                    b.ToTable("Models");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Picture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Path")
                        .IsRequired();

                    b.Property<int>("VehicleId");

                    b.HasKey("Id");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.RouteType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("RouteTypes");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int?>("CurrencyId");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.UserActivityLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionArguments");

                    b.Property<string>("ActionName")
                        .IsRequired();

                    b.Property<string>("AreaName");

                    b.Property<string>("ControllerName")
                        .IsRequired();

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("HttpMethod")
                        .IsRequired();

                    b.Property<string>("QueryString");

                    b.Property<string>("Url");

                    b.Property<string>("UserEmail")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserEmail");

                    b.ToTable("UserActivityLogs");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EngineHorsePower");

                    b.Property<string>("ExactModelname");

                    b.Property<double>("FuelConsumption");

                    b.Property<int>("FuelTypeId");

                    b.Property<int>("GearingTypeId");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ManufacturerId");

                    b.Property<int>("ModelId");

                    b.Property<int?>("PictureId");

                    b.Property<int>("TotalDistance");

                    b.Property<double>("TotalFuelAmount");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<int>("VehicleTypeId");

                    b.Property<int>("YearOfManufacture");

                    b.HasKey("Id");

                    b.HasIndex("FuelTypeId");

                    b.HasIndex("GearingTypeId");

                    b.HasIndex("ManufacturerId");

                    b.HasIndex("ModelId");

                    b.HasIndex("PictureId")
                        .IsUnique()
                        .HasFilter("[PictureId] IS NOT NULL");

                    b.HasIndex("UserId");

                    b.HasIndex("VehicleTypeId");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.VehicleType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("VehicleTypes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.CostEntry", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.CostEntryType", "CostEntryType")
                        .WithMany()
                        .HasForeignKey("CostEntryTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.Vehicle", "Vehicle")
                        .WithMany("CostEntries")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntry", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.FuelEntryType", "FuelEntryType")
                        .WithMany()
                        .HasForeignKey("FuelEntryTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.FuelType", "FuelType")
                        .WithMany("FuelEntries")
                        .HasForeignKey("FuelTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("VehicleCostsMonitor.Models.Vehicle", "Vehicle")
                        .WithMany("FuelEntries")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntryExtraFuelConsumer", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.ExtraFuelConsumer", "ExtraFuelConsumer")
                        .WithMany("FuelEntries")
                        .HasForeignKey("ExtraFuelConsumerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.FuelEntry", "FuelEntry")
                        .WithMany("ExtraFuelConsumers")
                        .HasForeignKey("FuelEntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.FuelEntryRouteType", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.FuelEntry", "FuelEntry")
                        .WithMany("Routes")
                        .HasForeignKey("FuelEntryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.RouteType", "RouteType")
                        .WithMany("FuelEntries")
                        .HasForeignKey("RouteTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Model", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.Manufacturer", "Manufacturer")
                        .WithMany("Models")
                        .HasForeignKey("ManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.User", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.Currency", "DisplayCurrency")
                        .WithMany()
                        .HasForeignKey("CurrencyId");
                });

            modelBuilder.Entity("VehicleCostsMonitor.Models.Vehicle", b =>
                {
                    b.HasOne("VehicleCostsMonitor.Models.FuelType", "FuelType")
                        .WithMany()
                        .HasForeignKey("FuelTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.GearingType", "GearingType")
                        .WithMany()
                        .HasForeignKey("GearingTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.Manufacturer", "Manufacturer")
                        .WithMany("Vehicles")
                        .HasForeignKey("ManufacturerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("VehicleCostsMonitor.Models.Model", "Model")
                        .WithMany("Vehicles")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("VehicleCostsMonitor.Models.Picture", "Picture")
                        .WithOne("Vehicle")
                        .HasForeignKey("VehicleCostsMonitor.Models.Vehicle", "PictureId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("VehicleCostsMonitor.Models.User", "User")
                        .WithMany("Vehicles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VehicleCostsMonitor.Models.VehicleType", "VehicleType")
                        .WithMany()
                        .HasForeignKey("VehicleTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
