create database PANDA
go

create table Patient
(
    PA_NhsNumber varchar(20)   not null
        constraint Patient_pk
            primary key,
    PA_Name      nvarchar(max) not null,
    PA_Postcode  varchar(20)   not null,
    PA_DOB       float         not null
)
go

create table Appointment
(
    AP_Identifier uniqueidentifier not null
        constraint Appointment_pk
            primary key,
    AP_Postcode   varchar(20)      not null,
    AP_Status     varchar(250)     not null,
    AP_Department varchar(250)     not null,
    AP_Duration   float            not null,
    PA_PatientId  varchar(20)      not null
        constraint Appointment_Patient_PA_NhsNumber_fk
            references Patient,
    AP_Time       datetimeoffset   not null,
    AP_Clinician  nvarchar(250)    not null
)
go

