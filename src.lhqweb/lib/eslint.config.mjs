import { FlatCompat } from "@eslint/eslintrc";
import js from "@eslint/js";
import path from "path";
import { fileURLToPath } from "url";

// mimic CommonJS variables -- not needed if using CommonJS
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const compat = new FlatCompat({
    recommendedConfig: js.configs.recommended, // optional unless you're using "eslint:recommended"
    baseDirectory: __dirname
});

export default [

    // mimic ESLintRC-style extends
    ...compat.extends(".eslintrc"),
];