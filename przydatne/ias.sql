
CREATE TABLE [dbo].[Kontakty](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[Imie] [varchar](50) NULL,
	[Nazwisko] [varchar](50) NULL,
	[Jednostka] [varchar](50) NULL,
	[Miejsce_pracy] [varchar](50) NULL,
	[Pion] [varchar](50) NULL,
	[Wydzial] [varchar](50) NULL,
	[Wydzial_podlegly] [varchar](50) NULL,
	[Stanowisko] [varchar](50) NULL,
	[Pokoj] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Telefon] [varchar](50) NULL,
	[Komorka] [varchar](50) NULL,
	[Login] [varchar](50) NULL,
	[Wewnetrzny] [varchar](50) NULL,
 CONSTRAINT [PK_Kontakty] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Upowaznienia]    Script Date: 2017-10-06 21:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Upowaznienia](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](250) NULL,
	[Nazwa_skrocona] [varchar](50) NULL,
	[Wniosek_nadania_upr] [varchar](350) NULL,
	[Nadajacy_upr] [varchar](350) NULL,
	[Prowadzacy_rejestr_uzyt] [varchar](350) NULL,
	[Wniosek_odebrania_upr] [varchar](350) NULL,
	[Odbierajacy_upr] [varchar](350) NULL,
	[Opiekun] [varchar](350) NULL,
	[Adres_email] [varchar](350) NULL,
	[Decyzja] [varchar](150) NULL,
	[Uwagi] [text] NULL,
 CONSTRAINT [PK_Upowaznienia] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[role](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[rola] [varchar](50) NULL,
 CONSTRAINT [PK_role] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[roleuzytkownika](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[id_roli] [bigint] NULL,
	[id_uzytkownika] [bigint] NULL,
 CONSTRAINT [PK_roleuzytkownika] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE TABLE [dbo].[UpowaznieniaPliki](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[id_upowaznienia] [bigint] NULL,
	[id_pliku]  [varchar](50) NULL,
	[nazwa] [varchar](50) NULL,
 CONSTRAINT [PK_UpowaznieniaPliki] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



