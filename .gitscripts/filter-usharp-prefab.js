// modified from: https://github.com/esnya/UdonRadioCommunications/blob/a43cc10fdb3e3577bbed9c7feb7df79aea3e2362/.gitscripts/filter-usharp-prefab.js

const fs = require("fs");

const guidPattern = '\\{fileID: .*(\r?\n     *type: [0-9]+)?\\}';
const pattern = new RegExp(
  [
    `    - target: ${guidPattern}\r?\n`,
    "      propertyPath: (serializedProgramAsset|serializationData\\..*)\r?\n",
    "      value:.*\r?\n",
    `      objectReference: ${guidPattern}\r?\n`,
    "|",
    `  serialized(Udon)?ProgramAsset: ${guidPattern}\r?\n`,
    "|",
    `  serializedProgramBytesString: .*\r?\n?`,
    "|",
    "    SerializedFormat: [02]\r?\n",
  ].join(""),
  "mg"
);

const input = process.argv[2]
  ? fs.createReadStream(process.argv[2])
  : process.stdin;

const chunks = [];
input.on("data", (chunk) => chunks.push(chunk));
input.on("end", () => {
  process.stdout.write(Buffer.concat(chunks).toString().replace(pattern, ""));
});