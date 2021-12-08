import { Strings, StringsMetadata } from "./Strings";
import Test = require("./Test");

$(() => {
    $.getJSON('Resources/Strings-metadata.json', (metadata: StringsMetadata) => {
        console.log(`Default language: ${metadata.default}`);
        console.log(`Available languages: ${metadata.languages}`);
    });

    $.getJSON('Resources/Strings.en.json', (strings: Strings) => {
        window['RS'] = strings;
        console.log('Loaded strings for "en" language: ', strings);
    });
});

setTimeout(() => {
    Test.testStrings();
}, 5000);