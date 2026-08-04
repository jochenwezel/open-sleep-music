import path from "node:path";
import { createRequire } from "node:module";

const require = createRequire(import.meta.url);
const sharp = require("sharp");

const directory = path.dirname(new URL(import.meta.url).pathname.replace(/^\/(.:)/, "$1"));
const outputDirectory = path.join(directory, "rendered");
const sizes = [16, 24, 32, 48, 256];
const variants = ["b", "c"];

const columnX = sizes.map((_, index) => 130 + index * 170);
const rowY = [200, 490];
const labels = sizes
  .map((size, index) => `<text x="${columnX[index]}" y="105" text-anchor="middle" font-family="Segoe UI" font-size="20" fill="#4b5563">${size}×${size}</text>`)
  .join("");

const background = Buffer.from(`
  <svg width="1000" height="650" xmlns="http://www.w3.org/2000/svg">
    <rect width="1000" height="650" fill="#f2f3f7"/>
    <text x="40" y="55" font-family="Segoe UI" font-size="28" fill="#111827">Open Sleep Music – native icon sizes</text>
    ${labels}
    <text x="40" y="210" font-family="Segoe UI" font-size="34" font-weight="bold" fill="#111827">B</text>
    <text x="40" y="500" font-family="Segoe UI" font-size="34" font-weight="bold" fill="#111827">C</text>
  </svg>`);

const composites = [];
for (const [row, variant] of variants.entries()) {
  for (const [column, size] of sizes.entries()) {
    const input = path.join(directory, `teddy-moon-${variant}-production.png`);
    const output = path.join(outputDirectory, `teddy-moon-${variant}-${size}.png`);
    await sharp(input).resize(size, size, { kernel: sharp.kernel.lanczos3 }).png().toFile(output);
    composites.push({
      input: await sharp(output).toBuffer(),
      left: columnX[column] - Math.floor(size / 2),
      top: rowY[row] - Math.floor(size / 2)
    });
  }
}

await sharp(background)
  .composite(composites)
  .png()
  .toFile(path.join(outputDirectory, "icon-size-comparison.png"));
