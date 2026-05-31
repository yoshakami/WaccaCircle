#!/usr/bin/env python3
"""
extract_sprites.py — cut the two wheel sprites from YOUR OWN screenshot.

Run this locally on a screenshot you captured; it writes sprites/core.png and
sprites/ring.png that WaccaWheelMenuRaster.cs loads. These crops are the game's
own rendered art, so keep them OUT of a public repo (see .gitignore note) —
generate them on each machine instead of committing them.

Usage:
    pip install pillow numpy
    python extract_sprites.py path/to/screenshot.png

Optional flags (defaults match a 1066x1913 portrait capture):
    --cx 532 --cy 908        wheel centre in the screenshot
    --core 182               tunnel core radius (outer edge of magenta rim)
    --ring-out 528           outer edge of the glitch ring
    --ring-in 392            inner edge of the glitch ring (hole)
    --out sprites            output folder

Tip to find the centre/radii: open the screenshot in any image editor, hover the
exact middle of the tunnel for cx/cy, and read the radius to the magenta rim
(core), the inner ring edge (ring-in), and the outer wheel edge (ring-out).
"""
import argparse, os, sys

def main():
    try:
        from PIL import Image, ImageDraw
        import numpy as np
    except ImportError:
        sys.exit("Install deps first:  pip install pillow numpy")

    ap = argparse.ArgumentParser()
    ap.add_argument("screenshot")
    ap.add_argument("--cx", type=int, default=532)
    ap.add_argument("--cy", type=int, default=908)
    ap.add_argument("--core", type=int, default=182)
    ap.add_argument("--ring-out", type=int, default=528)
    ap.add_argument("--ring-in", type=int, default=392)
    ap.add_argument("--out", default="sprites")
    a = ap.parse_args()

    im = Image.open(a.screenshot).convert("RGBA")
    os.makedirs(a.out, exist_ok=True)

    # --- core: filled circle, alpha outside the rim ---
    r = a.core
    crop = im.crop((a.cx - r, a.cy - r, a.cx + r, a.cy + r))
    mask = Image.new("L", (2 * r, 2 * r), 0)
    ImageDraw.Draw(mask).ellipse((1, 1, 2 * r - 1, 2 * r - 1), fill=255)
    arr = np.array(crop); arr[:, :, 3] = np.array(mask)
    Image.fromarray(arr).save(os.path.join(a.out, "core.png"))
    print("wrote", os.path.join(a.out, "core.png"), (2 * r, 2 * r))

    # --- ring: annulus, transparent centre hole ---
    ro, ri = a.ring_out, a.ring_in
    crop = im.crop((a.cx - ro, a.cy - ro, a.cx + ro, a.cy + ro))
    mask = Image.new("L", (2 * ro, 2 * ro), 0)
    d = ImageDraw.Draw(mask)
    d.ellipse((0, 0, 2 * ro, 2 * ro), fill=255)
    d.ellipse((ro - ri, ro - ri, ro + ri, ro + ri), fill=0)
    arr = np.array(crop); arr[:, :, 3] = np.array(mask)
    Image.fromarray(arr).save(os.path.join(a.out, "ring.png"))
    print("wrote", os.path.join(a.out, "ring.png"), (2 * ro, 2 * ro), "hole r=", ri)

if __name__ == "__main__":
    main()
