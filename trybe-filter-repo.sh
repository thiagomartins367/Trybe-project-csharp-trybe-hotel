### GIT FILTER-REPO ###

## NÃO EXECUTE ESSE SCRIPT DIRETAMENTE
## Esse script foi feito para uso do
## script 'trybe-publisher' fornecido 
## pela Trybe. 

[[ $# == 1 ]] && \
[[ $1 == "trybe-security-parameter" ]] && \
git filter-repo \
    --path .trybe \
    --path .github \
    --path trybe.yml \
    --path trybe-filter-repo.sh \
    --path src/TrybeHotel.Test.Test/ContextTest.cs \
    --path src/TrybeHotel.Test.Test/coverlet.sh \
    --path src/TrybeHotel.Test.Test/req01-TestModels.cs \
    --path src/TrybeHotel.Test.Test/req02-postUser.cs \
    --path src/TrybeHotel.Test.Test/req03-postLogin.cs \
    --path src/TrybeHotel.Test.Test/req04-postHotel.cs \
    --path src/TrybeHotel.Test.Test/req05-postRoom.cs \
    --path src/TrybeHotel.Test.Test/req06-deleteRoom.cs \
    --path src/TrybeHotel.Test.Test/req07-postBooking.cs \
    --path src/TrybeHotel.Test.Test/req08-getBooking.cs \
    --path src/TrybeHotel.Test.Test/req09-getUser.cs \
    --path src/TrybeHotel.Test.Test/req10-coverageTest.cs \
    --path src/TrybeHotel.Test.Test/Usings.cs \
    --path src/TrybeHotel.Test.Test/TrybeHotel.test.test.csproj \
    --path src/TrybeGames.Test.Test \
    --path img/der.png \
    --path docker-compose.yml \
    --path README.md \
    --path .gitignore \
    --invert-paths --force