"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const path = require("path");
const fs = require("fs");
const util = require("util");
const fsExist = util.promisify(fs.exists);
const fsReadFile = util.promisify(fs.readFile);
const resourcesFolder = path.join(__dirname, '../resources');
const modelName = 'strings';
const metadataFileName = path.join(resourcesFolder, `${modelName}-metadata.json`);
let metadata;
let defaltLanguage = 'en';
let currentLanguage = null;
const cache = {};
async function initialize() {
    if (await fsExist(metadataFileName)) {
        var metadataFile = await fsReadFile(metadataFileName, { encoding: 'utf8' });
        metadata = JSON.parse(metadataFile);
        if (metadata && metadata.default) {
            defaltLanguage = metadata.default;
            currentLanguage = defaltLanguage;
            await getStringsByLang(defaltLanguage);
        }
    }
}
exports.initialize = initialize;
function getLanguages() {
    return metadata.languages;
}
exports.getLanguages = getLanguages;
async function getStrings(url) {
    currentLanguage = getParameterByName('lang', url) || defaltLanguage;
    return await getStringsByLang(currentLanguage);
}
exports.getStrings = getStrings;
async function getStringsByLang(lang) {
    if (lang in cache) {
        return cache[lang];
    }
    let fileName = path.join(resourcesFolder, `strings.${currentLanguage}.json`);
    if (!await fsExist(fileName)) {
        currentLanguage = defaltLanguage;
        path.join(resourcesFolder, `strings.${currentLanguage}.json`);
    }
    let strings;
    if (await fsExist(fileName)) {
        var jsonFile = await fsReadFile(fileName, { encoding: 'utf8' });
        strings = JSON.parse(jsonFile);
    }
    else {
        strings = {};
    }
    cache[lang] = strings;
    return cache[lang];
}
function getParameterByName(name, url) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'), results = regex.exec(url);
    if (!results)
        return null;
    if (!results[2])
        return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
//# sourceMappingURL=stringsContext.js.map