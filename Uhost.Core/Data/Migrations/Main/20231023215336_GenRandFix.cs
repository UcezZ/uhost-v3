using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations.Main
{
    public partial class GenRandFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_random_chars(len INT)
RETURNS TEXT
AS $$
DECLARE val TEXT;
BEGIN
    SELECT
        string_agg(SUBSTR(characters, (RANDOM() * LENGTH(characters) + 1)::integer, 1), '') AS random_word
    FROM(VALUES('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_')) AS symbols(characters)
        JOIN generate_series(1, len) on 1 = 1
    INTO val;
    
    RETURN TRIM(val);
END
$$ LANGUAGE plpgsql;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_random_chars(len INT)
RETURNS TEXT
AS $$
DECLARE val TEXT;
BEGIN
    SELECT
        string_agg(SUBSTR(characters, (RANDOM()* LENGTH(characters) + 1)::integer, 1), '') AS random_word
    FROM(VALUES('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_')) AS symbols(characters)
        JOIN generate_series(1, len) on 1 = 1
    INTO val;
    
    RETURN val;
END
$$ LANGUAGE plpgsql;");
        }
    }
}
