using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Origami.Api.Infrastructure.Persistence.Scaffolded.Entities;

namespace Origami.Api.Infrastructure.Persistence.Scaffolded;

public partial class OrigamiScaffoldDbContext : DbContext
{
    public OrigamiScaffoldDbContext(DbContextOptions<OrigamiScaffoldDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CfcBook> CfcBooks { get; set; }

    public virtual DbSet<CfcCall> CfcCalls { get; set; }

    public virtual DbSet<CfcDiagram> CfcDiagrams { get; set; }

    public virtual DbSet<CfcResource> CfcResources { get; set; }

    public virtual DbSet<Creator> Creators { get; set; }

    public virtual DbSet<CreatorAlias> CreatorAliases { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<ModelPublication> ModelPublications { get; set; }

    public virtual DbSet<OrcModel> OrcModels { get; set; }

    public virtual DbSet<Publication> Publications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<CfcBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cfc_books_pkey");

            entity.ToTable("cfc_books");

            entity.HasIndex(e => e.Title, "cfc_books_title_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Author).HasColumnName("author");
            entity.Property(e => e.CloudinaryUrl).HasColumnName("cloudinary_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.PublishedDate).HasColumnName("published_date");
            entity.Property(e => e.ScrapedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scraped_at");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<CfcCall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cfc_calls_pkey");

            entity.ToTable("cfc_calls");

            entity.HasIndex(e => e.Title, "cfc_calls_title_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PostedOn).HasColumnName("posted_on");
            entity.Property(e => e.ScrapedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scraped_at");
            entity.Property(e => e.SubmissionDeadline).HasColumnName("submission_deadline");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<CfcDiagram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cfc_diagrams_pkey");

            entity.ToTable("cfc_diagrams");

            entity.HasIndex(e => e.Url, "cfc_diagrams_url_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.CloudinaryUrl).HasColumnName("cloudinary_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Creator).HasColumnName("creator");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.Downloads)
                .HasColumnType("jsonb")
                .HasColumnName("downloads");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.Language).HasColumnName("language");
            entity.Property(e => e.PaperSize).HasColumnName("paper_size");
            entity.Property(e => e.ScrapedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scraped_at");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<CfcResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cfc_resources_pkey");

            entity.ToTable("cfc_resources");

            entity.HasIndex(e => e.Url, "cfc_resources_url_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PostedOn).HasColumnName("posted_on");
            entity.Property(e => e.ResourceLinks)
                .HasColumnType("jsonb")
                .HasColumnName("resource_links");
            entity.Property(e => e.ScrapedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scraped_at");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<Creator>(entity =>
        {
            entity.HasKey(e => e.CreatorId).HasName("creators_pkey");

            entity.ToTable("creators");

            entity.HasIndex(e => e.NameNormalized, "uq_creator_name").IsUnique();

            entity.Property(e => e.CreatorId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("creator_id");
            entity.Property(e => e.Biography).HasColumnName("biography");
            entity.Property(e => e.BirthYear).HasColumnName("birth_year");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeathYear).HasColumnName("death_year");
            entity.Property(e => e.Language)
                .HasMaxLength(5)
                .HasColumnName("language");
            entity.Property(e => e.NameNormalized).HasColumnName("name_normalized");
            entity.Property(e => e.NameOriginal).HasColumnName("name_original");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CreatorAlias>(entity =>
        {
            entity.HasKey(e => e.AliasId).HasName("creator_aliases_pkey");

            entity.ToTable("creator_aliases");

            entity.HasIndex(e => e.AliasNormalized, "idx_creator_aliases_alias_norm");

            entity.HasIndex(e => e.CreatorId, "idx_creator_aliases_creator");

            entity.Property(e => e.AliasId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("alias_id");
            entity.Property(e => e.Alias).HasColumnName("alias");
            entity.Property(e => e.AliasNormalized).HasColumnName("alias_normalized");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.Language)
                .HasMaxLength(5)
                .HasColumnName("language");
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasColumnName("source");

            entity.HasOne(d => d.Creator).WithMany(p => p.CreatorAliases)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("fk_alias_creator");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("images_pkey");

            entity.ToTable("images");

            entity.HasIndex(e => e.Hash, "idx_images_hash");

            entity.HasIndex(e => e.ModelId, "idx_images_model");

            entity.Property(e => e.ImageId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("image_id");
            entity.Property(e => e.Angle)
                .HasMaxLength(20)
                .HasColumnName("angle");
            entity.Property(e => e.CloudinaryUrl).HasColumnName("cloudinary_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Hash)
                .HasMaxLength(128)
                .HasColumnName("hash");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValue(false)
                .HasColumnName("is_primary");
            entity.Property(e => e.LocalPath).HasColumnName("local_path");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.Model).WithMany(p => p.Images)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("fk_image_model");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("models_pkey");

            entity.ToTable("models");

            entity.HasIndex(e => e.CreatorId, "idx_models_creator");

            entity.HasIndex(e => e.ModelNameNormalized, "idx_models_name_norm");

            entity.Property(e => e.ModelId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("model_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.IsAbstract)
                .HasDefaultValue(false)
                .HasColumnName("is_abstract");
            entity.Property(e => e.ModelNameNormalized).HasColumnName("model_name_normalized");
            entity.Property(e => e.ModelNameOriginal).HasColumnName("model_name_original");
            entity.Property(e => e.PaperShape)
                .HasMaxLength(50)
                .HasColumnName("paper_shape");
            entity.Property(e => e.PaperToModelRatio)
                .HasMaxLength(20)
                .HasColumnName("paper_to_model_ratio");
            entity.Property(e => e.Pieces).HasColumnName("pieces");
            entity.Property(e => e.RecommendedPaperSize)
                .HasMaxLength(50)
                .HasColumnName("recommended_paper_size");
            entity.Property(e => e.SourceUrl).HasColumnName("source_url");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsesCutting)
                .HasDefaultValue(false)
                .HasColumnName("uses_cutting");
            entity.Property(e => e.UsesGlue)
                .HasDefaultValue(false)
                .HasColumnName("uses_glue");
            entity.Property(e => e.YearCreated).HasColumnName("year_created");

            entity.HasOne(d => d.Creator).WithMany(p => p.Models)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("fk_model_creator");
        });

        modelBuilder.Entity<ModelPublication>(entity =>
        {
            entity.HasKey(e => new { e.ModelId, e.PublicationId }).HasName("model_publications_pkey");

            entity.ToTable("model_publications");

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.PublicationId).HasColumnName("publication_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.InstructionType)
                .HasMaxLength(20)
                .HasColumnName("instruction_type");
            entity.Property(e => e.InstructionUrl).HasColumnName("instruction_url");
            entity.Property(e => e.PageNumber).HasColumnName("page_number");

            entity.HasOne(d => d.Model).WithMany(p => p.ModelPublications)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("fk_mp_model");

            entity.HasOne(d => d.Publication).WithMany(p => p.ModelPublications)
                .HasForeignKey(d => d.PublicationId)
                .HasConstraintName("fk_mp_publication");
        });

        modelBuilder.Entity<OrcModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orc_models_pkey");

            entity.ToTable("orc_models");

            entity.HasIndex(e => e.Category, "idx_orc_models_category");

            entity.HasIndex(e => e.CreatorRaw, "idx_orc_models_creator");

            entity.HasIndex(e => e.DiagramType, "idx_orc_models_diagram_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.CloudinaryUrl).HasColumnName("cloudinary_url");
            entity.Property(e => e.CreatorExpanded).HasColumnName("creator_expanded");
            entity.Property(e => e.CreatorRaw).HasColumnName("creator_raw");
            entity.Property(e => e.CreatorType)
                .HasDefaultValueSql("'unknown'::text")
                .HasColumnName("creator_type");
            entity.Property(e => e.DiagramIsArchived)
                .HasDefaultValue(false)
                .HasColumnName("diagram_is_archived");
            entity.Property(e => e.DiagramIsHostedOnOrc)
                .HasDefaultValue(false)
                .HasColumnName("diagram_is_hosted_on_orc");
            entity.Property(e => e.DiagramType).HasColumnName("diagram_type");
            entity.Property(e => e.DiagramUrl).HasColumnName("diagram_url");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsDollarBill)
                .HasDefaultValue(false)
                .HasColumnName("is_dollar_bill");
            entity.Property(e => e.ModelName).HasColumnName("model_name");
            entity.Property(e => e.ModelNameBase).HasColumnName("model_name_base");
            entity.Property(e => e.PageModified).HasColumnName("page_modified");
            entity.Property(e => e.PageTitle).HasColumnName("page_title");
            entity.Property(e => e.ScrapedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("scraped_at");
            entity.Property(e => e.SitemapLastmod).HasColumnName("sitemap_lastmod");
            entity.Property(e => e.SourcePageUrl).HasColumnName("source_page_url");
            entity.Property(e => e.Subcategory).HasColumnName("subcategory");
            entity.Property(e => e.VariantIndex).HasColumnName("variant_index");
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.HasKey(e => e.PublicationId).HasName("publications_pkey");

            entity.ToTable("publications");

            entity.HasIndex(e => e.Type, "idx_publications_type");

            entity.Property(e => e.PublicationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("publication_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("isbn");
            entity.Property(e => e.Language)
                .HasMaxLength(5)
                .HasColumnName("language");
            entity.Property(e => e.Publisher).HasColumnName("publisher");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
