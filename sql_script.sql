--Create Players table
create table if not exists players (
	id serial primary key,
	name varchar(50) unique not null
);

--Create relationships table
create table if not exists playerrelationships (
	playerId int,
	friendId int,
	constraint fk_player
	foreign key (playerId) references players(id),
	constraint fk_friend
	foreign key (friendId) references players(id)
);
--Make sure the relationships are unique
ALTER TABLE playerrelationships
ADD CONSTRAINT PR_UQ1 UNIQUE (playerId, friendId);

--Create avatars table
create table if not exists avatars (
	playerId int,
	name varchar(50) UNIQUE,
	constraint fk_player
	foreign key (playerId) references players(id)
);

--Make sure each player only has 1 avatar
ALTER TABLE avatars
ADD CONSTRAINT A_UQ1 UNIQUE (playerId, name);

--Seed data
insert into players (name) values ('Tom');
insert into players (name) values ('Bob');
insert into players (name) values ('Amy');
insert into players (name) values ('Kevin');
insert into players (name) values ('Mam');
insert into players (name) values ('Leo');
insert into players (name) values ('Mai');
insert into players (name) values ('Kristine');

insert into playerrelationships values (1, 2);
insert into playerrelationships values (1, 3);
insert into playerrelationships values (1, 9);
insert into playerrelationships values (1, 10);
insert into playerrelationships values (1, 11);
insert into playerrelationships values (9, 11);
insert into playerrelationships values (9, 10);
insert into playerrelationships values (10, 11);
insert into playerrelationships values (12, 11);

insert into avatars values (1, 'MaleRobot');
insert into avatars values (2, 'MaleRobot');
insert into avatars values (3, 'FemaleRobot');
insert into avatars values (9, 'MamAvatar');
insert into avatars values (10, 'LeoAvatar');
insert into avatars values (11, 'MaiAvatar');
insert into avatars values (12, 'KristineAvatar');