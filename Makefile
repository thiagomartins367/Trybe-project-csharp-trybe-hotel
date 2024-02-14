# ─────────────────────────────────────────────────────
# Docker DEV scripts

compose_up_dev: # Ex: make compose_up_dev
	docker-compose -f docker-compose.dev.yml up -d

compose_down_dev: # Ex: make compose_down_dev
	docker-compose -f docker-compose.dev.yml down --remove-orphans

compose_rm_images_dev: # Ex: make compose_rm_images_dev
	docker image rm dev_trybe-hotel test_trybe-hotel

# ─────────────────────────────────────────────────────
# Docker PROD scripts

compose_up_prod: # Ex: make compose_up_prod
	docker-compose -f docker-compose.prod.yml up -d

compose_down_prod: # Ex: make compose_down_prod
	docker-compose -f docker-compose.prod.yml down --remove-orphans

compose_rm_images_prod: # Ex: make compose_rm_images_prod
	docker image rm trybe-hotel entity-framework-trybe-hotel

# ─────────────────────────────────────────────────────
# Scripts

start: # Ex: make start
	dotnet run --project ./src/TrybeHotel/TrybeHotel.csproj

dev: # Ex: make dev
	dotnet watch run --project ./src/TrybeHotel/TrybeHotel.csproj

restore: # Ex: make restore
	dotnet restore ./src

format-file: # Ex: make file-path=src/TrybeHotel/Models/Booking.cs format-file
	dotnet format ./src/src.sln --verify-no-changes --report ./dotnet-format.json --include ./$(file-path) diagnostic

# ─────────────────────────────────────────────────────
# Test scripts

test-all: # Ex: make test-all
	dotnet test ./src/src.sln

test-all-filter: # Ex: make test-name=TestGetCities test-all-filter
	dotnet test ./src/src.sln --filter '$(test-name)'

test: # Ex: make test
	dotnet test ./src/TrybeHotel.Test/TrybeHotel.test.csproj

test-filter: # Ex: make test-name=TestGetCities test-filter
	dotnet test ./src/TrybeHotel.Test/TrybeHotel.test.csproj --filter '$(test-name)'

# ─────────────────────────────────────────────────────
# Entity Framework (EF) scripts

ef-add-migration: # Ex: make migration-name=MigrationName ef-add-migration
	dotnet ef migrations --project ./src/TrybeHotel/TrybeHotel.csproj add '$(migration-name)'

ef-list-migration: # Ex: make ef-list-migration
	dotnet ef migrations --project ./src/TrybeHotel/TrybeHotel.csproj list

ef-revert-until-migration: # Ex: make migration-name=MigrationName ef-revert-until-migration
	dotnet ef database --project ./src/TrybeHotel/TrybeHotel.csproj update '$(migration-name)'

ef-remove-migration: # (remove last pending migration) Ex: make ef-remove-migration
	dotnet ef migrations --project ./src/TrybeHotel/TrybeHotel.csproj remove

ef-force-remove-migration: # (revert and remove last migration) Ex: make ef-force-remove-migration
	dotnet ef migrations --project ./src/TrybeHotel/TrybeHotel.csproj remove --force

ef-database-update: # Ex: make ef-database-update
	dotnet ef database --project ./src/TrybeHotel/TrybeHotel.csproj update

# ─────────────────────────────────────────────────────