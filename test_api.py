"""
Comprehensive API test suite for BusinessReportsManager.
Tests auth, order CRUD, payments, financial calculations, report endpoints, and Excel export.
"""

import requests
import sys
from datetime import date, timedelta

BASE = "https://touragencyservice.onrender.com"
PASS = 0
FAIL = 0
ERRORS = []


def log_ok(msg):
    global PASS
    PASS += 1
    print(f"  ✓ {msg}")


def log_fail(msg):
    global FAIL
    FAIL += 1
    ERRORS.append(msg)
    print(f"  ✗ {msg}")


def check(condition, ok_msg, fail_msg):
    if condition:
        log_ok(ok_msg)
    else:
        log_fail(fail_msg)


def approx(a, b, tol=0.01):
    return abs(float(a) - float(b)) < tol


# ─────────────────────────────────────────────
# 1. AUTH
# ─────────────────────────────────────────────
print("\n═══ 1. AUTHENTICATION ═══")

# Login as supervisor (has all permissions)
r = requests.post(f"{BASE}/api/auth/login", json={
    "username": "supervisor",
    "password": "P@ssword1"
})
check(r.status_code == 200, "Supervisor login OK", f"Supervisor login failed: {r.status_code} {r.text}")
token = r.json().get("token", "")
check(len(token) > 0, "Got JWT token", "No JWT token returned")
headers = {"Authorization": f"Bearer {token}"}

# Login as employee
r = requests.post(f"{BASE}/api/auth/login", json={
    "username": "employee",
    "password": "P@ssword1"
})
check(r.status_code == 200, "Employee login OK", f"Employee login failed: {r.status_code}")
emp_token = r.json().get("token", "")
emp_headers = {"Authorization": f"Bearer {emp_token}"}

# Login as accountant
r = requests.post(f"{BASE}/api/auth/login", json={
    "username": "accountant",
    "password": "P@ssword1"
})
check(r.status_code == 200, "Accountant login OK", f"Accountant login failed: {r.status_code}")
acc_token = r.json().get("token", "")
acc_headers = {"Authorization": f"Bearer {acc_token}"}

# Whoami
r = requests.get(f"{BASE}/api/Debug/whoami", headers=headers)
check(r.status_code == 200, f"Whoami OK: {r.json()}", f"Whoami failed: {r.status_code}")

# Unauthenticated request should fail
r = requests.get(f"{BASE}/api/orders")
check(r.status_code == 401, "Unauthenticated GET /orders returns 401", f"Expected 401, got {r.status_code}")


# ─────────────────────────────────────────────
# 2. CREATE ORDER
# ─────────────────────────────────────────────
print("\n═══ 2. CREATE ORDER ═══")

SELL_PRICE = 5000.0
TOTAL_EXPENSE = 3200.0
TICKET_PRICE_USD = 400.0
TICKET_RATE = 2.75
HOTEL_PRICE_EUR = 600.0
HOTEL_RATE = 3.0
start = (date.today() + timedelta(days=30)).isoformat()
end = (date.today() + timedelta(days=37)).isoformat()

order_payload = {
    "party": {
        "fullName": "Test Customer",
        "email": "testcustomer@demo.local",
        "phone": "+995555111222",
        "personalNumber": "01234567890"
    },
    "tour": {
        "destination": "Italy Vacation",
        "startDate": start,
        "endDate": end,
        "passengerCount": 2,
        "supplier": {"name": "EuroTravel"},
        "airTickets": [
            {
                "countryFrom": "Georgia",
                "countryTo": "Italy",
                "cityFrom": "Tbilisi",
                "cityTo": "Rome",
                "flightDate": start,
                "price": {"currency": 2, "amount": TICKET_PRICE_USD, "exchangeRateToGel": TICKET_RATE}
            }
        ],
        "hotelBookings": [
            {
                "price": {"currency": 3, "amount": HOTEL_PRICE_EUR, "exchangeRateToGel": HOTEL_RATE}
            }
        ]
    },
    "source": "APITest",
    "sellPriceInGel": SELL_PRICE,
    "totalExpenseInGel": TOTAL_EXPENSE,
    "passengers": [
        {"fullName": "Alice Smith"},
        {"fullName": "Bob Smith"}
    ]
}

r = requests.post(f"{BASE}/api/orders", json=order_payload, headers=headers)
check(r.status_code == 201, "Order created (201)", f"Order create failed: {r.status_code} {r.text[:300]}")

order = r.json()
ORDER_ID = order.get("id")
check(ORDER_ID is not None, f"Order ID: {ORDER_ID}", "No order ID returned")
check(order.get("orderNumber", "").startswith("ORD-"), f"Order number: {order['orderNumber']}", "Bad order number format")
check(order.get("source") == "APITest", "Source correct", f"Source mismatch: {order.get('source')}")
check(approx(order.get("sellPriceInGel", 0), SELL_PRICE), f"Sell price = {SELL_PRICE}", f"Sell price mismatch: {order.get('sellPriceInGel')}")
check(approx(order.get("totalExpenseInGel", 0), TOTAL_EXPENSE), f"Total expense = {TOTAL_EXPENSE}", f"Expense mismatch: {order.get('totalExpenseInGel')}")
check(order.get("status") == 0, "Status = Open (0)", f"Status mismatch: {order.get('status')}")

# Verify party
party = order.get("party", {})
check(party.get("fullName") == "Test Customer", "Party name correct", f"Party name: {party.get('fullName')}")
check(party.get("email") == "testcustomer@demo.local", "Party email correct", f"Party email: {party.get('email')}")

# Verify tour
tour = order.get("tour", {})
check(tour.get("destination") == "Italy Vacation", "Tour destination correct", f"Tour dest: {tour.get('destination')}")
check(tour.get("passengerCount") == 2, "Passenger count = 2", f"Pax count: {tour.get('passengerCount')}")

# Verify air tickets
tickets = tour.get("airTickets", [])
check(len(tickets) == 1, "1 air ticket", f"Ticket count: {len(tickets)}")
if tickets:
    tp = tickets[0].get("price", {})
    expected_ticket_gel = TICKET_PRICE_USD * TICKET_RATE
    check(approx(tp.get("priceInGel", 0), expected_ticket_gel),
          f"Ticket priceInGel = {expected_ticket_gel} (USD {TICKET_PRICE_USD} × {TICKET_RATE})",
          f"Ticket priceInGel mismatch: {tp.get('priceInGel')}")

# Verify hotel bookings
hotels = tour.get("hotelBookings", [])
check(len(hotels) == 1, "1 hotel booking", f"Hotel count: {len(hotels)}")
if hotels:
    hp = hotels[0].get("price", {})
    expected_hotel_gel = HOTEL_PRICE_EUR * HOTEL_RATE
    check(approx(hp.get("priceInGel", 0), expected_hotel_gel),
          f"Hotel priceInGel = {expected_hotel_gel} (EUR {HOTEL_PRICE_EUR} × {HOTEL_RATE})",
          f"Hotel priceInGel mismatch: {hp.get('priceInGel')}")

# Verify passengers
pax = tour.get("passengers", [])
check(len(pax) == 2, "2 passengers", f"Passenger count: {len(pax)}")


# ─────────────────────────────────────────────
# 3. GET ORDER BY ID
# ─────────────────────────────────────────────
print("\n═══ 3. GET ORDER BY ID ═══")

r = requests.get(f"{BASE}/api/orders/{ORDER_ID}", headers=headers)
check(r.status_code == 200, "GET order by ID OK", f"GET by ID failed: {r.status_code}")
fetched = r.json()
check(fetched.get("id") == ORDER_ID, "ID matches", "ID mismatch")
check(approx(fetched.get("sellPriceInGel", 0), SELL_PRICE), "Sell price matches", "Sell price mismatch on GET")

# Non-existent order
r = requests.get(f"{BASE}/api/orders/00000000-0000-0000-0000-000000000000", headers=headers)
check(r.status_code == 404, "Non-existent order returns 404", f"Expected 404, got {r.status_code}")


# ─────────────────────────────────────────────
# 4. PAYMENTS & FINANCIAL CALCULATIONS
# ─────────────────────────────────────────────
print("\n═══ 4. PAYMENTS & FINANCIAL CALCULATIONS ═══")

# Add payment 1: 1000 GEL
PAY1_AMOUNT = 1000.0
PAY1_RATE = 1.0
r = requests.post(f"{BASE}/api/payments/{ORDER_ID}", json={
    "price": {"currency": 1, "amount": PAY1_AMOUNT, "exchangeRateToGel": PAY1_RATE},
    "bankName": "TBC",
    "paidDate": date.today().isoformat()
}, headers=headers)
check(r.status_code == 200, f"Payment 1 added: {PAY1_AMOUNT} GEL", f"Payment 1 failed: {r.status_code} {r.text[:200]}")
pay1 = r.json()
PAY1_ID = pay1.get("id")

# Add payment 2: 500 USD at rate 2.75
PAY2_AMOUNT = 500.0
PAY2_RATE = 2.75
r = requests.post(f"{BASE}/api/payments/{ORDER_ID}", json={
    "price": {"currency": 2, "amount": PAY2_AMOUNT, "exchangeRateToGel": PAY2_RATE},
    "bankName": "BOG",
    "paidDate": date.today().isoformat()
}, headers=headers)
check(r.status_code == 200, f"Payment 2 added: {PAY2_AMOUNT} USD", f"Payment 2 failed: {r.status_code} {r.text[:200]}")
pay2 = r.json()
PAY2_ID = pay2.get("id")

# Calculate expected totals
TOTAL_PAID_GEL = (PAY1_AMOUNT * PAY1_RATE) + (PAY2_AMOUNT * PAY2_RATE)  # 1000 + 1375 = 2375
EXPECTED_PROFIT = SELL_PRICE - TOTAL_EXPENSE  # 5000 - 3200 = 1800
EXPECTED_LEFT = SELL_PRICE - TOTAL_PAID_GEL   # 5000 - 2375 = 2625

# Customer paid total
r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/customer-paid", headers=headers)
check(r.status_code == 200, "GET customer-paid OK", f"customer-paid failed: {r.status_code}")
paid_val = r.json()
check(approx(paid_val, TOTAL_PAID_GEL),
      f"Customer paid = {TOTAL_PAID_GEL} GEL ({PAY1_AMOUNT}×{PAY1_RATE} + {PAY2_AMOUNT}×{PAY2_RATE})",
      f"Customer paid mismatch: got {paid_val}, expected {TOTAL_PAID_GEL}")

# Expenses
r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/expenses", headers=headers)
check(r.status_code == 200, "GET expenses OK", f"expenses failed: {r.status_code}")
expenses_val = r.json()
check(approx(expenses_val, TOTAL_EXPENSE),
      f"Expenses = {TOTAL_EXPENSE}",
      f"Expenses mismatch: got {expenses_val}, expected {TOTAL_EXPENSE}")

# Profit
r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/profit", headers=headers)
check(r.status_code == 200, "GET profit OK", f"profit failed: {r.status_code}")
profit_val = r.json()
check(approx(profit_val, EXPECTED_PROFIT),
      f"Profit = {EXPECTED_PROFIT} (sell {SELL_PRICE} - expense {TOTAL_EXPENSE})",
      f"Profit mismatch: got {profit_val}, expected {EXPECTED_PROFIT}")

# Supplier owed
r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/supplier-owed", headers=headers)
check(r.status_code == 200, "GET supplier-owed OK", f"supplier-owed failed: {r.status_code}")
owed_val = r.json()
check(approx(owed_val, TOTAL_EXPENSE),
      f"Supplier owed = {TOTAL_EXPENSE}",
      f"Supplier owed mismatch: got {owed_val}, expected {TOTAL_EXPENSE}")

# Financial summary
r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/financial-summary", headers=headers)
check(r.status_code == 200, "GET financial-summary OK", f"financial-summary failed: {r.status_code}")
fs = r.json()
check(approx(fs.get("sellPriceInGel", 0), SELL_PRICE), f"Summary: sellPrice = {SELL_PRICE}", f"Summary sellPrice: {fs.get('sellPriceInGel')}")
check(approx(fs.get("totalExpenseInGel", 0), TOTAL_EXPENSE), f"Summary: totalExpense = {TOTAL_EXPENSE}", f"Summary expense: {fs.get('totalExpenseInGel')}")
check(approx(fs.get("totalPaidInGel", 0), TOTAL_PAID_GEL), f"Summary: totalPaid = {TOTAL_PAID_GEL}", f"Summary paid: {fs.get('totalPaidInGel')}")
check(approx(fs.get("profitInGel", 0), EXPECTED_PROFIT), f"Summary: profit = {EXPECTED_PROFIT}", f"Summary profit: {fs.get('profitInGel')}")

customer_remaining = fs.get("customerRemainingInGel", 0)
check(approx(customer_remaining, EXPECTED_LEFT),
      f"Summary: customerRemaining = {EXPECTED_LEFT} (sell {SELL_PRICE} - paid {TOTAL_PAID_GEL})",
      f"Summary remaining: got {customer_remaining}, expected {EXPECTED_LEFT}")

cash_flow = fs.get("cashFlowInGel", 0)
expected_cash_flow = TOTAL_PAID_GEL - TOTAL_EXPENSE  # 2375 - 3200 = -825
check(approx(cash_flow, expected_cash_flow),
      f"Summary: cashFlow = {expected_cash_flow} (paid {TOTAL_PAID_GEL} - expense {TOTAL_EXPENSE})",
      f"Summary cashFlow: got {cash_flow}, expected {expected_cash_flow}")


# ─────────────────────────────────────────────
# 5. ORDER REPORT (GET /api/orders)
# ─────────────────────────────────────────────
print("\n═══ 5. ORDER REPORT LIST ═══")

r = requests.get(f"{BASE}/api/orders", headers=headers)
check(r.status_code == 200, "GET /api/orders OK", f"GET orders failed: {r.status_code}")
report = r.json()
check(isinstance(report, list) and len(report) > 0, f"Got {len(report)} order(s) in report", "Empty report list")

our_report = next((o for o in report if o.get("id") == ORDER_ID), None)
check(our_report is not None, "Our test order found in report", "Test order missing from report")

if our_report:
    # Verify report fields match our data
    check(our_report.get("numberOfPax") == 2, "Report: numberOfPax = 2", f"Report pax: {our_report.get('numberOfPax')}")
    check("Alice Smith" in our_report.get("listOfPassengers", ""), "Report: passengers include Alice Smith", f"Report passengers: {our_report.get('listOfPassengers')}")
    check("Bob Smith" in our_report.get("listOfPassengers", ""), "Report: passengers include Bob Smith", f"Report passengers: {our_report.get('listOfPassengers')}")
    check(approx(our_report.get("grossPrice", 0), SELL_PRICE), f"Report: grossPrice = {SELL_PRICE}", f"Report grossPrice: {our_report.get('grossPrice')}")
    check(approx(our_report.get("totalExpenses", 0), TOTAL_EXPENSE), f"Report: totalExpenses = {TOTAL_EXPENSE}", f"Report totalExpenses: {our_report.get('totalExpenses')}")
    check(approx(our_report.get("profit", 0), EXPECTED_PROFIT), f"Report: profit = {EXPECTED_PROFIT}", f"Report profit: {our_report.get('profit')}")
    check(approx(our_report.get("paidByClient", 0), TOTAL_PAID_GEL), f"Report: paidByClient = {TOTAL_PAID_GEL}", f"Report paidByClient: {our_report.get('paidByClient')}")
    check(approx(our_report.get("leftToPay", 0), EXPECTED_LEFT), f"Report: leftToPay = {EXPECTED_LEFT}", f"Report leftToPay: {our_report.get('leftToPay')}")
    check(our_report.get("currency") == "GEL", "Report: currency = GEL", f"Report currency: {our_report.get('currency')}")

    # Verify removed fields are NOT present
    check("tourName" not in our_report, "Report: tourName removed", "Report still has tourName!")
    check("rentName" not in our_report, "Report: rentName removed", "Report still has rentName!")
    check("managerName" not in our_report, "Report: managerName removed", "Report still has managerName!")
    check("ticketPrice" not in our_report, "Report: ticketPrice removed (collapsed into totalExpenses)", "Report still has ticketPrice!")
    check("hotelPrice" not in our_report, "Report: hotelPrice removed (collapsed into totalExpenses)", "Report still has hotelPrice!")


# ─────────────────────────────────────────────
# 6. SEARCH & FILTER ENDPOINTS
# ─────────────────────────────────────────────
print("\n═══ 6. SEARCH & FILTER ENDPOINTS ═══")

# By status
r = requests.get(f"{BASE}/api/orders/status/0", headers=headers)
check(r.status_code == 200, "GET /orders/status/0 (Open) OK", f"Status filter failed: {r.status_code}")
status_report = r.json()
check(isinstance(status_report, list), "Status filter returns list", "Not a list")
found = any(o.get("id") == ORDER_ID for o in status_report)
check(found, "Test order found in Open status filter", "Test order missing from Open status filter")

# Search by tour name
r = requests.get(f"{BASE}/api/orders/search", params={"tourName": "Italy"}, headers=headers)
check(r.status_code == 200, "GET /orders/search?tourName=Italy OK", f"Search failed: {r.status_code}")
search_report = r.json()
check(isinstance(search_report, list), "Search returns list", "Not a list")
found = any(o.get("id") == ORDER_ID for o in search_report)
check(found, "Test order found via tour name search", "Test order missing from search results")


# ─────────────────────────────────────────────
# 7. EXCEL EXPORT
# ─────────────────────────────────────────────
print("\n═══ 7. EXCEL EXPORT ═══")

r = requests.get(f"{BASE}/api/orders/export-excel", headers=headers)
check(r.status_code == 200, "GET /orders/export-excel OK", f"Excel export failed: {r.status_code}")
check(r.headers.get("content-type", "").startswith("application/vnd.openxmlformats"), "Content-Type is xlsx", f"Content-Type: {r.headers.get('content-type')}")
check(len(r.content) > 500, f"Excel file size: {len(r.content)} bytes", "Excel file too small")

disp = r.headers.get("content-disposition", "")
check("Report sample" in disp or "report" in disp.lower() or ".xlsx" in disp,
      f"Filename in Content-Disposition: {disp}", f"Unexpected Content-Disposition: {disp}")


# ─────────────────────────────────────────────
# 8. ACCOUNTING COMMENT
# ─────────────────────────────────────────────
print("\n═══ 8. ACCOUNTING COMMENT ═══")

r = requests.patch(f"{BASE}/api/orders/{ORDER_ID}/accounting-comment",
                   json={"comment": "Test accounting note"},
                   headers=acc_headers)
check(r.status_code == 204, "Accountant updated comment OK", f"Comment update failed: {r.status_code} {r.text[:200]}")

r = requests.get(f"{BASE}/api/orders/{ORDER_ID}", headers=headers)
check(r.json().get("accountingComment") == "Test accounting note",
      "Comment persisted correctly", f"Comment: {r.json().get('accountingComment')}")


# ─────────────────────────────────────────────
# 9. STATUS CHANGE
# ─────────────────────────────────────────────
print("\n═══ 9. STATUS CHANGE ═══")

r = requests.patch(f"{BASE}/api/orders/{ORDER_ID}/status", params={"status": 1}, headers=headers)
check(r.status_code == 204, "Status changed to Finalized (1)", f"Status change failed: {r.status_code}")

r = requests.get(f"{BASE}/api/orders/{ORDER_ID}", headers=headers)
check(r.json().get("status") == 1, "Order status is now Finalized", f"Status: {r.json().get('status')}")

# Verify it shows up in Finalized filter
r = requests.get(f"{BASE}/api/orders/status/1", headers=headers)
found = any(o.get("id") == ORDER_ID for o in r.json())
check(found, "Test order found in Finalized status filter", "Missing from Finalized filter")


# ─────────────────────────────────────────────
# 10. PAYMENT DELETION
# ─────────────────────────────────────────────
print("\n═══ 10. PAYMENT DELETION ═══")

r = requests.delete(f"{BASE}/api/payments/payment/{PAY2_ID}", headers=headers)
check(r.status_code == 204, "Payment 2 deleted", f"Payment delete failed: {r.status_code}")

# Recalculate after deletion
NEW_TOTAL_PAID = PAY1_AMOUNT * PAY1_RATE  # just payment 1 = 1000
NEW_LEFT = SELL_PRICE - NEW_TOTAL_PAID     # 5000 - 1000 = 4000

r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/customer-paid", headers=headers)
paid_after = r.json()
check(approx(paid_after, NEW_TOTAL_PAID),
      f"After delete: paid = {NEW_TOTAL_PAID}",
      f"After delete paid mismatch: got {paid_after}, expected {NEW_TOTAL_PAID}")

r = requests.get(f"{BASE}/api/payments/{ORDER_ID}/financial-summary", headers=headers)
fs2 = r.json()
check(approx(fs2.get("customerRemainingInGel", 0), NEW_LEFT),
      f"After delete: remaining = {NEW_LEFT}",
      f"After delete remaining: {fs2.get('customerRemainingInGel')}")

# Report should also reflect the change
r = requests.get(f"{BASE}/api/orders", headers=headers)
our_report2 = next((o for o in r.json() if o.get("id") == ORDER_ID), None)
if our_report2:
    check(approx(our_report2.get("paidByClient", 0), NEW_TOTAL_PAID),
          f"Report after delete: paidByClient = {NEW_TOTAL_PAID}",
          f"Report paidByClient after delete: {our_report2.get('paidByClient')}")
    check(approx(our_report2.get("leftToPay", 0), NEW_LEFT),
          f"Report after delete: leftToPay = {NEW_LEFT}",
          f"Report leftToPay after delete: {our_report2.get('leftToPay')}")


# ─────────────────────────────────────────────
# 11. TOURS CONTROLLER REMOVED
# ─────────────────────────────────────────────
print("\n═══ 11. TOURS CONTROLLER REMOVED ═══")

r = requests.get(f"{BASE}/api/tours", headers=headers)
check(r.status_code == 404, "GET /api/tours returns 404 (removed)", f"Expected 404, got {r.status_code}")


# ─────────────────────────────────────────────
# 12. ROLE-BASED ACCESS
# ─────────────────────────────────────────────
print("\n═══ 12. ROLE-BASED ACCESS ═══")

# Employee cannot delete orders (Supervisor only)
r = requests.delete(f"{BASE}/api/orders/{ORDER_ID}", headers=emp_headers)
check(r.status_code == 403, "Employee cannot delete order (403)", f"Expected 403 for employee delete, got {r.status_code}")

# Employee cannot change status (Supervisor/Accountant only)
r = requests.patch(f"{BASE}/api/orders/{ORDER_ID}/status", params={"status": 0}, headers=emp_headers)
check(r.status_code == 403, "Employee cannot change status (403)", f"Expected 403 for employee status change, got {r.status_code}")


# ─────────────────────────────────────────────
# 13. CLEANUP — DELETE TEST ORDER
# ─────────────────────────────────────────────
print("\n═══ 13. CLEANUP ═══")

r = requests.delete(f"{BASE}/api/orders/{ORDER_ID}", headers=headers)
check(r.status_code == 204, "Test order deleted", f"Delete failed: {r.status_code}")

r = requests.get(f"{BASE}/api/orders/{ORDER_ID}", headers=headers)
check(r.status_code == 404, "Deleted order returns 404", f"Expected 404 after delete, got {r.status_code}")


# ─────────────────────────────────────────────
# SUMMARY
# ─────────────────────────────────────────────
print("\n" + "═" * 50)
print(f"  PASSED: {PASS}")
print(f"  FAILED: {FAIL}")
print("═" * 50)

if ERRORS:
    print("\nFailed checks:")
    for e in ERRORS:
        print(f"  ✗ {e}")

sys.exit(1 if FAIL > 0 else 0)
