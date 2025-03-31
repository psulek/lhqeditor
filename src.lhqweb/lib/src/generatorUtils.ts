import { DOMParser as xmlDomParser } from '@xmldom/xmldom';
import * as xpath from 'xpath';
import * as zodToJsonSchema from 'zod-to-json-schema';
import { fromZodError, createMessageBuilder } from 'zod-validation-error';

import { type LhqModel, LhqModelSchema } from './api/schemas';
import { isNullOrEmpty, tryJsonParse, tryRemoveBOM } from './utils';
import type { LhqValidationResult } from './types';

let DOMParser: typeof globalThis.DOMParser;

export function validateLhqModel(data: LhqModel | string): LhqValidationResult {
    if (typeof data === 'string') {
        const parseResult = tryJsonParse(data, true);

        if (!parseResult.success) {
            return { success: false, error: parseResult.error };
        }

        data = parseResult.data as LhqModel;
    }

    if (data === undefined || data === null || typeof data !== 'object') {
        return { success: false, error: 'Specified "data" must be an object!' };
    }

    const parseResult = LhqModelSchema.safeParse(data);
    const success = parseResult.success && !isNullOrEmpty(parseResult.data);

    let error: string | undefined = undefined;
    if (!parseResult.success) {
        const messageBuilder = createMessageBuilder({
            prefix: '',
            prefixSeparator: '',
            issueSeparator: '\n'
        });
        const err = fromZodError(parseResult.error, { messageBuilder });
        error = err.toString();
    }

    return { success, error, model: success ? parseResult.data : undefined };
}

export function generateLhqSchema(): string {
    const jsonSchema = zodToJsonSchema.zodToJsonSchema(LhqModelSchema, {
        name: 'LhqModel',
        $refStrategy: 'root'
    });

    return JSON.stringify(jsonSchema, null, 2);
}

// Constants and helper variables
const itemGroupTypes = ['None', 'Compile', 'Content', 'EmbeddedResource'];
const itemGroupTypesAttrs = ['Include', 'Update'];
const csProjectXPath = '//ns:ItemGroup/ns:##TYPE##[@##ATTR##="##FILE##"]';
const xpathRootNamespace = 'string(//ns:RootNamespace)';
const xpathAssemblyName = 'string(//ns:AssemblyName)';

export type CSharpNamespaceInfo = {
    csProjectFileName: string;
    t4FileName: string;
    namespace: string | undefined;
    referencedLhqFile: boolean;
    referencedT4File: boolean;
};

export function getRootNamespaceFromCsProj(lhqModelFileName: string, t4FileName: string,
    csProjectFileName: string, csProjectFileContent: string): CSharpNamespaceInfo | undefined {
    let referencedLhqFile = false;
    let referencedT4File = false;

    if (isNullOrEmpty(csProjectFileName) || isNullOrEmpty(csProjectFileContent)) {
        return undefined;
    }

    let rootNamespace: string | undefined;

    try {
        const fileContent = tryRemoveBOM(csProjectFileContent);
        if (typeof window !== "undefined" && typeof window.DOMParser !== "undefined") {
            // Running in a browser, use built-in DOMParser
            DOMParser = window.DOMParser;
        } else {
            DOMParser = xmlDomParser as any;
        }

        const doc = new DOMParser().parseFromString(fileContent, 'text/xml');
        const rootNode = doc as unknown as Node;
        const ns = doc.documentElement?.namespaceURI || '';

        const xpathSelect = xpath.useNamespaces({ ns: ns });

        const findFileElement = function (fileName: string): Element | undefined {
            for (const itemGroupType of itemGroupTypes) {
                for (const attr of itemGroupTypesAttrs) {
                    const xpathQuery = csProjectXPath.replace('##TYPE##', itemGroupType)
                        .replace('##ATTR##', attr)
                        .replace('##FILE##', fileName);

                    const element = xpathSelect(xpathQuery, rootNode, true) as Element;
                    if (element) {
                        return element;
                    }
                }
            }
            return undefined;
        }

        rootNamespace = xpathSelect(xpathRootNamespace, rootNode, true) as string;

        referencedLhqFile = findFileElement(lhqModelFileName) != undefined;
        const t4FileElement = findFileElement(t4FileName);
        if (t4FileElement) {
            referencedT4File = true;
            const dependentUpon = t4FileElement.getElementsByTagNameNS(ns, 'DependentUpon')[0]?.textContent;
            if (dependentUpon && dependentUpon === lhqModelFileName) {
                referencedLhqFile = true;
            }

            const customToolNamespace = t4FileElement.getElementsByTagNameNS(ns, 'CustomToolNamespace')[0]?.textContent;
            if (customToolNamespace) {
                rootNamespace = customToolNamespace;
            }
        }

        if (!rootNamespace) {
            rootNamespace = xpathSelect(xpathAssemblyName, rootNode, true) as string;
        }
    } catch (e) {
        console.error('Error getting root namespace.', e);
        rootNamespace = undefined;
    }

    return { csProjectFileName, t4FileName, namespace: rootNamespace, referencedLhqFile, referencedT4File };
}