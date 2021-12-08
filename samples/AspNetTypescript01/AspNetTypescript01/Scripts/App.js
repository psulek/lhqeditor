$(function () {
    $.getJSON('Resources/Strings-metadata.json', function (metadata) {
        console.log("Default language: " + metadata.default);
        console.log("Available languages: " + metadata.languages);
    });
    $.getJSON('Resources/Strings.en.json', function (strings) {
        console.log('Loaded strings for "en" language: ', strings);
    });
});
//# sourceMappingURL=App.js.map