import path from "node:path";
import { createRequire } from "node:module";

const require = createRequire(import.meta.url);
const sharp = require("sharp");
const directory = path.dirname(new URL(import.meta.url).pathname.replace(/^\/(.:)/, "$1"));
const size = 1024;
const cornerRadius = 270;
const navy = "#0B1020";

const roundedMask = Buffer.from(`
  <svg width="${size}" height="${size}" xmlns="http://www.w3.org/2000/svg">
    <rect width="${size}" height="${size}" rx="${cornerRadius}" fill="white"/>
  </svg>`);

for (const variant of ["b", "c"]) {
  const input = path.join(directory, `teddy-moon-${variant}-soft.png`);
  const resized = await sharp(input)
    .trim({ background: "#ffffff", threshold: 8 })
    .resize(size, size, { fit: "fill", kernel: sharp.kernel.lanczos3 })
    .png()
    .toBuffer();
  const masked = await sharp(resized)
    .composite([{ input: roundedMask, blend: "dest-in" }])
    .png()
    .toBuffer();

  await sharp({ create: { width: size, height: size, channels: 4, background: navy } })
    .composite([{ input: masked }])
    .png()
    .toFile(path.join(directory, `teddy-moon-${variant}-production.png`));
}
