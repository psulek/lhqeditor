import fs from 'node:fs';
import fsp from 'node:fs/promises';
import { Generator } from '../generator';
import { HbsTemplateManager } from '../hbsManager';
import path from 'node:path';
import { safeJsonParse } from '../utils';
import { LhqModel } from '../model/api';
import { GeneratedFile } from '../types';

export async function generateFromLhq(lhqFileName: string): Promise<void> {
    HbsTemplateManager.registerTemplate('NetCoreResxCsharp01', await readHbsFile('NetCoreResxCsharp01.hbs'))

    const lhqFile = await fsp.readFile(lhqFileName, { encoding: 'utf-8' });
    const model = safeJsonParse<LhqModel>(lhqFile);

    Generator.initialize();
    const generator = new Generator();
    const result = generator.generate({ fileName: lhqFileName, model }, { 'namespace': 'Root' });
    console.log(`Generated ${result.generatedFiles.length} files.\n------------\n`);

    //result.generatedFiles.forEach(x => console.log(`[${x.FileName}]\n${x.getContent(true)}`));
    const output = path.resolve(__dirname, '../../temp');
    result.generatedFiles.forEach((file) => {
        saveGenFile(generator, file, output);
        console.log(`Saved file ${file.fileName}.`);
    });
}

async function saveGenFile(generator: Generator, generatedFile: GeneratedFile, outputPath?: string): Promise<void> {
    const content = generator.getFileContent(generatedFile, true);
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