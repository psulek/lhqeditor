import fs from 'node:fs';
import fsp from 'node:fs/promises';
import { Generator, HbsTemplateManager } from '../generator';
import path from 'node:path';
import { safeJsonParse } from '../utils';
import { LhqModel } from '../model/api';
import { GeneratedFile } from '../generatedFile';

export async function generateFromLhq(lhqFileName: string): Promise<void> {
    HbsTemplateManager.registerTemplate('NetCoreResxCsharp01', await readHbsFile('NetCoreResxCsharp01.hbs'))

    const lhqFile = await fsp.readFile(lhqFileName, { encoding: 'utf-8' });
    const model = safeJsonParse<LhqModel>(lhqFile);

    Generator.initialize();
    const result = Generator.generate({ fileName: lhqFileName, model }, {'namespace': 'Root'});
    console.log(`Generated ${result.generatedFiles.length} files.\n------------\n`);

    //result.generatedFiles.forEach(x => console.log(`[${x.FileName}]\n${x.getContent(true)}`));
    const output = path.resolve(__dirname, '../../temp');
    result.generatedFiles.forEach(function (x) {
            saveGenFile(x, output);
            console.log(`Saved file ${x.fileName}.`);
        });
}

async function saveGenFile(generatedFile: GeneratedFile, outputPath?: string): Promise<void> {
    const content = generatedFile.getContent(true);
    const bom = generatedFile.bom ? '\uFEFF' : '';
    const encodedText = Buffer.from(bom + content, 'utf8');

    const fileName = !outputPath ? generatedFile.fileName : path.join(outputPath, generatedFile.fileName);
    const dir = path.dirname(fileName);
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }

    await fsp.writeFile(fileName, encodedText, { encoding: 'utf8' });


    // async function write(): Promise<void> {
    //     return new Promise((resolve, reject) => {
    //         const writeStream = fs.createWriteStream(fileName, { encoding: 'utf8' });
    //         writeStream.write(encodedText);
    //         writeStream.once('finish', resolve);
    //         writeStream.once('error', reject);
    //         writeStream.end();
    //     });
    // }

    // await write();

    // Alternatively, using async/await with fs.promises:
    // import * as fsPromises from 'fs/promises';
    // await fsPromises.writeFile(fileName, encodedText, { encoding: 'utf8' });
}

async function readHbsFile(fileName: string): Promise<string> {
    const file = path.resolve(__dirname, '..', 'hbs', fileName);
    return await fsp.readFile(file, { encoding: 'utf-8' });
}