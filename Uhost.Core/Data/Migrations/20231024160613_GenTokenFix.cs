using Microsoft.EntityFrameworkCore.Migrations;

namespace Uhost.Core.Data.Migrations
{
    public partial class GenTokenFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_random_chars(len INT, chars TEXT DEFAULT NULL)
RETURNS TEXT
AS $$
DECLARE val TEXT;
BEGIN
    IF chars IS NULL
    THEN
        SELECT 
            'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_'
        INTO
            chars;
    END IF;
    
    SELECT
        string_agg(SUBSTR(characters, (RANDOM() * LENGTH(characters) + 1)::integer, 1), '') AS random_word
    FROM(VALUES(chars)) AS symbols(characters)
        JOIN generate_series(1, len) on 1 = 1
    INTO val;
    
    RETURN TRIM(val);
END
$$ LANGUAGE plpgsql;");
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_file_token()
RETURNS TEXT
AS $$
DECLARE token TEXT;
BEGIN
    SELECT gen_random_chars(32, '0123456789abcdef') INTO token;

    IF EXISTS (SELECT ""Token"" FROM ""Files"" WHERE ""Token"" = token) 
    THEN
        RETURN gen_file_token();
    ELSE
        RETURN token;
    END IF;
END
$$ LANGUAGE plpgsql;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION gen_file_token()
RETURNS TEXT
AS $$
DECLARE token TEXT;
BEGIN
    SELECT gen_random_chars(32) INTO token;

    IF EXISTS (SELECT ""Token"" FROM ""Files"" WHERE ""Token"" = token) 
    THEN
        RETURN gen_file_token();
    ELSE
        RETURN token;
    END IF;
END
$$ LANGUAGE plpgsql;");
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
    }
}
