import { Strings, StringsMetadata } from "./Strings";

$(() => {
    $.getJSON('Resources/Strings-metadata.json', (metadata: StringsMetadata) => {
        console.log(`Default language: ${metadata.default}`);
        console.log(`Available languages: ${metadata.languages}`);
    });

    $.getJSON('Resources/Strings.en.json', (strings: Strings) => {
        console.log('Loaded strings for "en" language: ', strings);
    });
});