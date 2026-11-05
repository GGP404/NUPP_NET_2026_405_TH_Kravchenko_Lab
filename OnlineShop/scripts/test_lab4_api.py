#!/usr/bin/env python3
"""Run live smoke tests against OnlineShop.REST and write an HTML report.

Usage:
    python scripts/test_lab4_api.py
    python scripts/test_lab4_api.py --base-url http://localhost:5101

Open the generated HTML in a browser and use Print -> Save as PDF to create
an evidence file with the actual HTTP responses for the lab submission.
"""
from __future__ import annotations

import argparse
import datetime as dt
import html
import json
import urllib.error
import urllib.request
from pathlib import Path
from typing import Any


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--base-url", default="http://localhost:5101", help="Base URL of OnlineShop.REST")
    parser.add_argument("--output", default="Lab4_API_Test_Report.html", help="HTML report file path")
    args = parser.parse_args()
    base_url = args.base_url.rstrip("/")
    results: list[dict[str, str]] = []

    def send(method: str, path: str, payload: dict[str, Any] | None = None) -> tuple[int | None, str, Any | None]:
        body = None if payload is None else json.dumps(payload, ensure_ascii=False).encode("utf-8")
        headers = {"Accept": "application/json"}
        if body is not None:
            headers["Content-Type"] = "application/json; charset=utf-8"
        request = urllib.request.Request(base_url + path, data=body, headers=headers, method=method)
        try:
            with urllib.request.urlopen(request, timeout=10) as response:
                status = response.status
                raw = response.read().decode("utf-8", errors="replace")
        except urllib.error.HTTPError as exc:
            status = exc.code
            raw = exc.read().decode("utf-8", errors="replace")
        except (urllib.error.URLError, TimeoutError, OSError) as exc:
            return None, str(exc), None
        try:
            parsed = json.loads(raw) if raw.strip() else None
        except json.JSONDecodeError:
            parsed = None
        return status, raw, parsed

    def record(label: str, method: str, path: str, expected: int, payload: dict[str, Any] | None = None) -> Any | None:
        actual, body, parsed = send(method, path, payload)
        results.append({
            "test": label,
            "method": method,
            "path": path,
            "expected": str(expected),
            "actual": "connection error" if actual is None else str(actual),
            "result": "PASS" if actual == expected else "FAIL",
            "body": body[:2500] if body else "(empty response body)",
        })
        print(f"[{results[-1]['result']}] {method} {path}: expected {expected}, got {results[-1]['actual']}")
        return parsed

    record("List products", "GET", "/api/products?page=1&pageSize=10", 200)
    product = record("Create laptop", "POST", "/api/products", 201, {
        "name": "Lab 4 test laptop",
        "price": 42999,
        "type": "Laptop",
        "brand": "Lenovo",
        "ram": 16,
        "processor": "AMD Ryzen 7",
    })
    product_id = product.get("id") if isinstance(product, dict) else None
    if product_id:
        record("Read created product", "GET", f"/api/products/{product_id}", 200)
        record("Update created product", "PUT", f"/api/products/{product_id}", 204, {
            "name": "Lab 4 test laptop updated",
            "price": 41999,
            "type": "Laptop",
            "brand": "Lenovo",
            "ram": 32,
            "processor": "AMD Ryzen 7",
        })
    else:
        results.append({"test": "Read created product", "method": "GET", "path": "/api/products/{id}", "expected": "200", "actual": "skipped", "result": "SKIP", "body": "Product creation did not return an id."})
        results.append({"test": "Update created product", "method": "PUT", "path": "/api/products/{id}", "expected": "204", "actual": "skipped", "result": "SKIP", "body": "Product creation did not return an id."})
    record("Missing product returns 404", "GET", "/api/products/00000000-0000-0000-0000-000000000000", 404)

    record("List customers", "GET", "/api/customers?page=1&pageSize=10", 200)
    customer = record("Create customer", "POST", "/api/customers", 201, {
        "name": "Lab 4 test customer",
        "email": f"lab4-{dt.datetime.now().strftime('%Y%m%d%H%M%S')}@example.com",
        "age": 22,
    })
    customer_id = customer.get("id") if isinstance(customer, dict) else None
    if customer_id:
        record("Read created customer", "GET", f"/api/customers/{customer_id}", 200)
        record("Update created customer", "PUT", f"/api/customers/{customer_id}", 204, {
            "name": "Lab 4 test customer updated",
            "email": customer.get("email", "lab4@example.com"),
            "age": 23,
        })
    else:
        results.append({"test": "Read created customer", "method": "GET", "path": "/api/customers/{id}", "expected": "200", "actual": "skipped", "result": "SKIP", "body": "Customer creation did not return an id."})
        results.append({"test": "Update created customer", "method": "PUT", "path": "/api/customers/{id}", "expected": "204", "actual": "skipped", "result": "SKIP", "body": "Customer creation did not return an id."})
    record("Missing customer returns 404", "GET", "/api/customers/00000000-0000-0000-0000-000000000000", 404)

    if product_id:
        record("Delete created product", "DELETE", f"/api/products/{product_id}", 204)
    if customer_id:
        record("Delete created customer", "DELETE", f"/api/customers/{customer_id}", 204)

    passed = sum(row["result"] == "PASS" for row in results)
    failed = sum(row["result"] == "FAIL" for row in results)
    skipped = sum(row["result"] == "SKIP" for row in results)
    generated = dt.datetime.now().astimezone().strftime("%Y-%m-%d %H:%M:%S %Z")
    rows = []
    for row in results:
        row_class = "pass" if row["result"] == "PASS" else "fail" if row["result"] == "FAIL" else "skip"
        rows.append(
            "<tr class='{cls}'><td>{test}</td><td>{method}</td><td><code>{path}</code></td>"
            "<td>{expected}</td><td>{actual}</td><td><strong>{result}</strong></td><td><pre>{body}</pre></td></tr>".format(
                cls=row_class, **{key: html.escape(value) for key, value in row.items()}
            )
        )
    report = f"""<!doctype html>
<html lang="uk"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>Lab 4 API test report</title><style>
body{{font:14px/1.5 Arial,sans-serif;color:#182230;margin:28px}}h1{{margin-bottom:4px}}.meta{{color:#526171}}.summary{{display:flex;gap:10px;margin:20px 0;flex-wrap:wrap}}.pill{{border:1px solid #cbd5e1;border-radius:8px;padding:10px 14px}}table{{border-collapse:collapse;width:100%;font-size:12px}}th,td{{border:1px solid #cbd5e1;padding:8px;text-align:left;vertical-align:top}}th{{background:#eff3f8}}.pass td:nth-child(6){{color:#187044}}.fail td:nth-child(6){{color:#b42318}}.skip td:nth-child(6){{color:#945d00}}pre{{white-space:pre-wrap;overflow-wrap:anywhere;max-width:420px;margin:0}}code{{overflow-wrap:anywhere}}@media print{{body{{margin:10mm}}tr{{page-break-inside:avoid}}.summary{{gap:5px}}.pill{{padding:5px 8px}}}}
</style></head><body>
<h1>Лабораторна робота №4 — протокол перевірки REST API</h1>
<p class="meta">Дата виконання: {html.escape(generated)} · Середовище: {html.escape(base_url)}</p>
<div class="summary"><div class="pill">Усього сценаріїв: <strong>{len(results)}</strong></div><div class="pill">PASS: <strong>{passed}</strong></div><div class="pill">FAIL: <strong>{failed}</strong></div><div class="pill">SKIP: <strong>{skipped}</strong></div></div>
<p>У таблиці наведено фактичні HTTP-статуси та тіла відповідей, отримані під час виконання скрипта проти запущеного сервера.</p>
<table><thead><tr><th>Перевірка</th><th>HTTP</th><th>Маршрут</th><th>Очікувалось</th><th>Фактично</th><th>Результат</th><th>Тіло відповіді / помилка</th></tr></thead><tbody>{''.join(rows)}</tbody></table>
</body></html>"""
    output = Path(args.output).resolve()
    output.write_text(report, encoding="utf-8")
    print(f"\nHTML report written to: {output}")
    print("Open the file in a browser and choose Print -> Save as PDF for the lab submission.")
    return 1 if failed else 0


if __name__ == "__main__":
    raise SystemExit(main())
