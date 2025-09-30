import path from 'node:path';

import fs from 'node:fs/promises';
import fse from 'fs-extra';
import { expect, use } from 'chai';
import chaiBytes from 'chai-bytes';
use(chaiBytes);

import { isNullOrEmpty, tryRemoveBOM } from '../src/utils.js';

export type TestFolders = {
    cwd: string;
    hbs: string;
    snapshots: string;
    csproj: string;
    data: string;
    templates: string;
}

export type FilePath = {
    full: string;
    relative?: string;
    exist?: boolean;
}

let _folders: TestFolders | undefined;

export function folders(): TestFolders {
    if (!_folders) {
        const dir = __dirname;
        const hbs = path.resolve(dir, '..', 'hbs');
        const dirData = path.join(dir, 'data');
        const dirCsProj = path.join(dirData, 'csprojs');
        const dirTemplates = path.join(dirData, 'templates');

        _folders = Object.freeze({
            cwd: dir,
            hbs: hbs,
            snapshots: path.join(dir, 'snapshots'),
            csproj: dirCsProj,
            data: dirData,
            templates: dirTemplates
        });
    }
    return _folders;
}

export function delay(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function safeReadFile(fileName: string): Promise<string> {
    if (!(await fse.pathExists(fileName))) {
        throw new Error(`File '${fileName}' not found.`);
    }

    const content = await fse.readFile(fileName, { encoding: 'utf-8' });
    return isNullOrEmpty(content) ? '' : tryRemoveBOM(content);
}

export async function verifyFile(fileName: string, value: string | Buffer | Record<string, unknown>): Promise<void> {
    const binary = Buffer.isBuffer(value);
    if (typeof value === 'object' && !binary) {
        value = JSON.stringify(value, null, 2);
    }

    await fse.ensureDir(path.dirname(fileName));

    if (await fse.pathExists(fileName)) {
        const snapshot = binary ? await fse.readFile(fileName) : await fse.readFile(fileName, { encoding: 'utf-8' });
        if (binary) {
            expect(value).to.equalBytes(snapshot);
        } else {
            expect(value).to.equal(snapshot, `Received value does not match verified value.`);
        }
    } else {
        if (binary) {
            await fs.writeFile(fileName, value as Buffer);
        } else {
            await fse.writeFile(fileName, value as any, { encoding: 'utf-8' });
        }
        //throw new Error(`Test '${title}' failed. Snapshot file '${snapshotFile}' created. Please run the test again.`);
        throw new Error(`Snapshot file '${fileName}' created. Please run the test again.`);
    }
}

export async function verify(/* suite: Mocha.Suite */ snapshotFolder: string, ident: string, value: string | Buffer | Record<string, unknown>): Promise<void> {
    const dir = path.join(folders().snapshots, snapshotFolder);
    //await fse.ensureDir(dir);
    const binary = Buffer.isBuffer(value);
    const snapshotFile = path.join(dir, `${ident}.${binary ? 'bin' : 'txt'}`);

    await verifyFile(snapshotFile, value);
}


export function createFilePath(filePath: string, rootFolderOrFilePath: string | FilePath, fileMustExist: boolean, formatRelative: boolean = true): FilePath {
    if (isNullOrEmpty(filePath)) {
        throw new Error(`Parameter 'inputPath' could not be undefined or empty!`);
    }

    const rootFolder = typeof rootFolderOrFilePath === 'string' ? rootFolderOrFilePath : rootFolderOrFilePath.full;
    const isAbsolute = path.isAbsolute(filePath);
    const full = isAbsolute ? filePath : path.join(rootFolder, filePath);
    if (path.relative(rootFolder, full).startsWith('../')) {
        throw new Error(`File '${filePath}' is outside of root folder '${rootFolder}'!`);
    }

    let relative = isAbsolute ? path.relative(rootFolder, filePath) : filePath;
    //relative = relative.replace('\\', '/');
    relative = relative.replace(/\\/g, '/');
    if (formatRelative) {
        relative = formatRelativePath(relative);
    }

    if (relative.startsWith('../')) {
        throw new Error(`File '${filePath}' is outside of root folder '${rootFolder}'!`);
    }

    const exist = fse.pathExistsSync(full);
    if (!exist && fileMustExist) {
        throw new Error(`File '${filePath}' does not exist!`);
    }

    return { full, relative: relative, exist };
}

export const formatRelativePath = (p: string): string => {
    const h = p.slice(0, 1);
    const res = h === '.' ? p : (h === '/' ? `.${p}` : `./${p}`);
    return res.replace('\\', '/');
};
