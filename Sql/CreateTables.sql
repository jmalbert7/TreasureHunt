IF OBJECT_ID('Games') IS NOT NULL
	DROP TABLE Games
	DROP TABLE Clues
	DROP TABLE Hunts
	DROP TABLE Users

------------------------Users---------------------------
CREATE TABLE Users
(
	UserId INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
	Username VARCHAR(20) NOT NULL,
	HashedPassword VARCHAR(100) NOT NULL
)

INSERT Users
	(Username, HashedPassword)
VALUES
	('jmalbert7', '55B9C21A6822552E6F69CE874BF72041')

--------------------------------------------------------

------------------------Hunts---------------------------
CREATE TABLE Hunts
(
	HuntId INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
	UserId INT FOREIGN KEY REFERENCES Users(UserId),
	HuntName VARCHAR(50) NOT NULL,
	GeneralLocation VARCHAR(100) NOT NULL
)
--------------------------------------------------------

------------------------Clues---------------------------
CREATE TABLE Clues
(
	ClueId INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
	HuntId INT FOREIGN KEY REFERENCES Hunts(HuntId),
	FirstFlag BIT DEFAULT 0,
	LastFlag BIT DEFAULT 0,
	NextClueId INT FOREIGN KEY REFERENCES Clues(ClueId),
	Location VARCHAR(200),
	Riddle VARCHAR(500)
)
--------------------------------------------------------

------------------------Games---------------------------
CREATE TABLE Games
(
	GameId INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
	UserId INT FOREIGN KEY REFERENCES Users(UserId),
	ClueId INT FOREIGN KEY REFERENCES Clues(ClueId)
)
--------------------------------------------------------