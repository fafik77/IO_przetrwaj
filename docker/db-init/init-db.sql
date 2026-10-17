-- Initialize schemas and PostGIS extension
CREATE SCHEMA IF NOT EXISTS postgis;
CREATE EXTENSION IF NOT EXISTS postgis SCHEMA postgis;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
