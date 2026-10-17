FROM postgres:18

ENV POSTGIS_MAJOR=3

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR \
        postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR-scripts \
    && rm -rf /var/lib/apt/lists/*

COPY db-init/init-db.sql /docker-entrypoint-initdb.d/
