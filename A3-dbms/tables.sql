use lab3
go
drop table consultation
drop table animal
drop table vet

create table Animal(
	aid integer not null primary key,
	name varchar(30) not null,
	type_animal varchar(30),
	age integer
)

create table Vet(
	vid integer not null primary key,
	name varchar(30) not null, 
	phone_number varchar(30)
)

create table Consultation(
	aid integer not null,
	vid integer not null,
	week_day varchar(30),

	foreign key (aid) references Animal(aid) on delete cascade on update cascade,
	foreign key (vid) references Vet(vid) on delete cascade on update cascade,
	constraint pk_consultation primary key (aid, vid)
)

