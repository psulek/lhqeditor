import * as path from 'path';
import * as fs from 'fs';
import * as util from 'util';
import { Istrings, IstringsMetadata } from "./strings";

const fsExist = util.promisify(fs.exists);
const fsReadFile = util.promisify(fs.readFile);

const resourcesFolder = path.join(__dirname, '../resources');
const modelName = 'strings';
const metadataFileName = path.join(resourcesFolder, `${modelName}-metadata.json`);

let metadata: IstringsMetadata;
let defaltLanguage = 'en';
let currentLanguage = null;
const cache: { [lang: string]: Istrings } = {};

export async function initialize(): Promise<void> {
    if (await fsExist(metadataFileName)) {
        var metadataFile = await fsReadFile(metadataFileName, { encoding: 'utf8' });
        metadata = JSON.parse(metadataFile) as IstringsMetadata;
        if (metadata && metadata.default) {
            defaltLanguage = metadata.default;
            currentLanguage = defaltLanguage;
            await getStringsByLang(defaltLanguage);
        }
    }
}

export function getLanguages(): string[] {
    return metadata.languages;
}

export async function getStrings(url: string): Promise<Istrings> {
    currentLanguage = getParameterByName('lang', url) || defaltLanguage;
    return await getStringsByLang(currentLanguage);
}

async function getStringsByLang(lang: string): Promise<Istrings> {
    if (lang in cache) {
        return cache[lang];
    }

    let fileName = path.join(resourcesFolder, `strings.${currentLanguage}.json`);
    if (!await fsExist(fileName)) {
        currentLanguage = defaltLanguage;
        path.join(resourcesFolder, `strings.${currentLanguage}.json`);
    }

    let strings: Istrings;
    if (await fsExist(fileName)) {
        var jsonFile = await fsReadFile(fileName, { encoding: 'utf8' });
        strings = JSON.parse(jsonFile) as Istrings;
    } else {
        strings = {} as any;
    }

    cache[lang] = strings;
    return cache[lang];
}

    
function getParameterByName(name: string, url: string) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}