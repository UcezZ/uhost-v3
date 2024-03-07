using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations
{
    public partial class Debloat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS unaccent");
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION debloat(src TEXT) RETURNS TEXT
AS $$
BEGIN
    RETURN regexp_replace(REPLACE(LOWER(unaccent(src)), 'й', 'и'), '[\s\,\.\-_\+\(\)\!\/\""\u0027\^\$%]', '', 'g');
END;
$$ LANGUAGE plpgsql IMMUTABLE;");

            migrationBuilder.Sql("DROP INDEX \"IX_Videos_Name\"");
            migrationBuilder.Sql("CREATE INDEX \"IX_Videos_Name_debloat\" ON \"Videos\" USING GIN (debloat(\"Name\") gin_trgm_ops)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Videos_Name_debloat\"");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS debloat(TEXT)");
            migrationBuilder.Sql("CREATE INDEX \"IX_Videos_Name\" ON \"Videos\" USING gin (\"Name\" gin_trgm_ops)");
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS unaccent");
        }
    }
}
