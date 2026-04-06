
/*
-- SQLite
CREATE TABLE TestDbExtensions (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    IntValue        INTEGER,
    DoubleValue     REAL,
    FloatValue      REAL,
    LongValue       INTEGER,
    BoolValue       TEXT,
    StringValue     TEXT,
    SplitValue      TEXT,
    JsonValue       TEXT,
    DateValue       TEXT,
    GuidValue       TEXT,
    BlobValue       BLOB
);

INSERT INTO TestDbExtensions
(IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(42, 3.14159, 2.5, 9999999999, 'TRUE', 'Hello World', 'A,B,C,D', '{"name":"Alice","age":30}', '2024-12-25 15:30:00', '550e8400-e29b-41d4-a716-446655440000', X'48656C6C6F426C6F62');


INSERT INTO TestDbExtensions
(IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
('123', '12.34', '8.8', '1234567890123', '1', 'Test String', 'One|Two|Three', '{"active":true}', '25-Dec-2024', 'd94fca1e-771d-4b2a-9ee7-7fd7d049bf16', X'0102030405');



INSERT INTO TestDbExtensions
(IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);



INSERT INTO TestDbExtensions
(IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(-99, '0.0001', '15.75', '-123456789', 'Y', 'Another Value', '10;20;30;40', '{"items":[1,2,3]}', '2024-01-01', '5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b', X'CAFEBABE');

-- MariaDB / MySql
CREATE TABLE TestDbExtensions (
    Id              INT PRIMARY KEY,
    IntValue        INT,
    DoubleValue     DOUBLE,
    FloatValue      FLOAT,
    LongValue       BIGINT,
    BoolValue       VARCHAR(10),
    StringValue     VARCHAR(255),
    SplitValue      VARCHAR(255),
    JsonValue       TEXT,
    DateValue       DATETIME,
    GuidValue       VARCHAR(50),
    BlobValue       BLOB
);

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(1, 42, 3.14159, 2.5, 9999999999, 'TRUE', 'Hello World', 'A,B,C,D', '{"name":"Alice","age":30}', '2024-12-25 15:30:00', '550e8400-e29b-41d4-a716-446655440000', X'48656C6C6F426C6F62');

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(2, '123', '12.34', '8.8', '1234567890123', '1', 'Test String', 'One|Two|Three', '{"active":true}', '2024-12-25', 'd94fca1e-771d-4b2a-9ee7-7fd7d049bf16', X'0102030405');

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(4, -99, '0.0001', '15.75', '-123456789', 'Y', 'Another Value', '10;20;30;40', '{"items":[1,2,3]}', '2024-01-01', '5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b', X'CAFEBABE');


-- Oracle
CREATE TABLE TestDbExtensions (
    Id              NUMBER,
    IntValue        NUMBER,
    DoubleValue     BINARY_DOUBLE,
    FloatValue      BINARY_FLOAT,
    LongValue       NUMBER,
    BoolValue       VARCHAR2(10),
    StringValue     VARCHAR2(4000),
    SplitValue      VARCHAR2(4000),
    JsonValue       CLOB,
    DateValue       DATE,
    GuidValue       VARCHAR2(36),
    BlobValue       BLOB
);

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(1, 42, 3.14159, 2.5, 9999999999, 'TRUE', 'Hello World', 'A,B,C,D',
 '{"name":"Alice","age":30}',
 TO_DATE('2024-12-25 15:30:00', 'YYYY-MM-DD HH24:MI:SS'),
 '550e8400-e29b-41d4-a716-446655440000',
 HEXTORAW('48656C6C6F426C6F62'));

  INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(2, 123, 12.34, 8.8, 1234567890123, '1', 'Test String', 'One|Two|Three',
 '{"active":true}',
 TO_DATE('25-Dec-2024', 'DD-MON-YYYY'),
 'd94fca1e-771d-4b2a-9ee7-7fd7d049bf16',
 HEXTORAW('0102030405'));

  INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(4, -99, 0.0001, 15.75, -123456789, 'Y', 'Another Value', '10;20;30;40',
 '{"items":[1,2,3]}',
 TO_DATE('20240101', 'YYYYMMDD'),
 '5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b',
 HEXTORAW('CAFEBABE'));

 -- SQL SERVER
CREATE TABLE TestDbExtensions (
    Id              INT PRIMARY KEY,  
    IntValue        INT,
    DoubleValue     FLOAT,
    FloatValue      REAL,
    LongValue       BIGINT,
    BoolValue       NVARCHAR(10),
    StringValue     NVARCHAR(4000),
    SplitValue      NVARCHAR(4000),
    JsonValue       NVARCHAR(MAX),
    DateValue       DATETIME,
    GuidValue       UNIQUEIDENTIFIER,
    BlobValue       VARBINARY(MAX)
);

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(1, 42, 3.14159, 2.5, 9999999999, 'TRUE', 'Hello World', 'A,B,C,D',
 '{"name":"Alice","age":30}',
 '2024-12-25T15:30:00', 
 '550e8400-e29b-41d4-a716-446655440000',
 0x48656C6C6F426C6F62);


 INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(2, 123, 12.34, 8.8, 1234567890123, '1', 'Test String', 'One|Two|Three',
 '{"active":true}',
 '2024-12-25T00:00:00',
 'd94fca1e-771d-4b2a-9ee7-7fd7d049bf16',
 0x0102030405);

 INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

INSERT INTO TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(4, -99, 0.0001, 15.75, -123456789, 'Y', 'Another Value', '10;20;30;40',
 '{"items":[1,2,3]}',
 '2024-01-01T00:00:00',
 '5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b',
 0xCAFEBABE);

  -- IRIS
  Class Test.TestDbExtensions Extends (%Persistent, %XML.Adaptor)
{

Property Id As %Integer;

Property IntValue As %Integer;

Property DoubleValue As %Double;

Property FloatValue As %Float;

Property LongValue As %Integer;

Property BoolValue As %String(MAXLEN = 10);

Property StringValue As %String(MAXLEN = 4000);

Property SplitValue As %String(MAXLEN = 4000);

Property JsonValue As %String(MAXLEN = 32000);

Property DateValue As %TimeStamp;

Property GuidValue As %String(MAXLEN = 36);

Property BlobValue As %Binary;
}

INSERT INTO Test.TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(
    1,
    42,
    3.14159,
    2.5,
    9999999999,
    'TRUE',
    'Hello World',
    'A,B,C,D',
    '{"name":"Alice","age":30}',
    '2024-12-25 15:30:00',
    '550e8400-e29b-41d4-a716-446655440000',
    CAST('' AS BINARY)   -- empty BLOB for testing
)

INSERT INTO Test.TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(
    2,
    123,
    12.34,
    8.8,
    1234567890123,
    '1',
    'Test String',
    'One|Two|Three',
    '{"active":true}',
    '2024-12-25 00:00:00',
    'd94fca1e-771d-4b2a-9ee7-7fd7d049bf16',
    CAST('' AS BINARY)
)

INSERT INTO Test.TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(
    3,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL
)

INSERT INTO Test.TestDbExtensions
(Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue, JsonValue, DateValue, GuidValue, BlobValue)
VALUES
(
    4,
    -99,
    0.0001,
    15.75,
    -123456789,
    'Y',
    'Another Value',
    '10;20;30;40',
    '{"items":[1,2,3]}',
    '2024-01-01 00:00:00',
    '5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b',
    CAST('' AS BINARY)
)

    Set obj = ##class(Test.TestDbExtensions).%OpenId(4)

    Set hex = "48656C6C6F426C6F62"  ; "HelloBlob"
    Set len = $LENGTH(hex)
    Set data = ""

    For i = 1:2:len {
        Set byte = $ZHEX($EXTRACT(hex, i, i+1))
        Set data = data _ $CHAR(byte)
    }

    ; Assign binary directly — no %New()
    Set obj.BlobValue = data

    Do obj.%Save()


    --Oracle Function
    CREATE OR REPLACE FUNCTION TESTFUNCTIONDATASTRING 
(
  p_id1 IN VARCHAR2 
) RETURN VARCHAR2 AS 

    return_data varchar2 (100);

BEGIN
    SELECT StringValue INTO return_data FROM TESTDBEXTENSIONS WHERE ID = p_id1;
  RETURN return_data;
  
  EXCEPTION
  WHEN NO_DATA_FOUND THEN
    RETURN NULL;
  WHEN TOO_MANY_ROWS THEN
    RETURN NULL;
END TESTFUNCTIONDATASTRING;

--MariaDb & MySql function

DROP FUNCTION IF EXISTS TESTFUNCTIONDATASTRING;

CREATE FUNCTION TESTFUNCTIONDATASTRING(p_id VARCHAR(100))
RETURNS VARCHAR(100)
DETERMINISTIC
BEGIN
    DECLARE return_data VARCHAR(100);

    SELECT StringValue
    INTO return_data
    FROM TestDbExtensions
    WHERE ID = p_id
    LIMIT 1;

    RETURN return_data;
END;

--SqlServer
CREATE FUNCTION TESTFUNCTIONDATASTRING (@p_id VARCHAR(100))
RETURNS VARCHAR(100)
AS
BEGIN
    DECLARE @return_data VARCHAR(100);

    SELECT @return_data = StringValue
    FROM TESTDBEXTENSIONS
    WHERE ID = @p_id;

    RETURN @return_data;
END;
GO

*/