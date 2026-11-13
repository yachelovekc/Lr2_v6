<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template match="/StudentPerformanceSystem">
    <html>
      <head>
        <meta charset="utf-8" />
        <title>Звіт: Успішність студентів (Варіант 6)</title>
        <style>
          body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f6f9;
            color: #1f2937;
            margin: 24px;
          }
          h1 {
            color: #1e3a8a;
            border-bottom: 2px solid #2563eb;
            padding-bottom: 8px;
          }
          .subtitle {
            color: #4b5563;
            margin-bottom: 20px;
          }
          .student-card {
            background: #ffffff;
            border: 1px solid #d1d5db;
            border-left: 5px solid #2563eb;
            border-radius: 6px;
            padding: 16px;
            margin-bottom: 20px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
          }
          .student-header {
            font-size: 1.15em;
            font-weight: 700;
            color: #111827;
            margin-bottom: 8px;
          }
          .meta-grid {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 0.92em;
            color: #374151;
            margin-bottom: 12px;
          }
          .badge {
            background-color: #e0e7ff;
            color: #1e40af;
            padding: 3px 8px;
            border-radius: 4px;
            font-weight: 600;
          }
          table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 8px;
            font-size: 0.92em;
          }
          th, td {
            border: 1px solid #e5e7eb;
            padding: 8px 10px;
            text-align: left;
          }
          th {
            background-color: #1e3a8a;
            color: #ffffff;
          }
          tr:nth-child(even) {
            background-color: #f9fafb;
          }
          .grade-high {
            color: #047857;
            font-weight: bold;
          }
        </style>
      </head>
      <body>
        <h1>Інформаційна система «Успішність студентів»</h1>
        <div class="subtitle">
          Університет: <strong><xsl:value-of select="@university" /></strong> |
          Всього студентів у вибірці: <strong><xsl:value-of select="count(Student)" /></strong>
        </div>

        <xsl:for-each select="Student">
          <div class="student-card">
            <div class="student-header">
              <xsl:value-of select="@FullName" />
              (<span class="badge"><xsl:value-of select="@Id" /></span>)
            </div>
            <div class="meta-grid">
              <span><strong>Факультет:</strong> <xsl:value-of select="@Faculty" /></span>
              <span><strong>Кафедра:</strong> <xsl:value-of select="@Department" /></span>
              <span><strong>Курс:</strong> <xsl:value-of select="@Course" /></span>
              <span><strong>Група:</strong> <xsl:value-of select="@Group" /></span>
              <xsl:if test="@Specialty">
                <span><strong>Спеціальність:</strong> <xsl:value-of select="@Specialty" /></span>
              </xsl:if>
              <xsl:if test="@StudyForm">
                <span><strong>Форма навчання:</strong> <xsl:value-of select="@StudyForm" /></span>
              </xsl:if>
              <xsl:if test="@Scholarship">
                <span><strong>Стипендія:</strong> <xsl:value-of select="@Scholarship" /></span>
              </xsl:if>
            </div>

            <table>
              <thead>
                <tr>
                  <th>Навчальна дисципліна</th>
                  <th>Семестр</th>
                  <th>Кредити ECTS</th>
                  <th>Форма контролю</th>
                  <th>Бал (100)</th>
                  <th>Оцінка</th>
                  <th>Викладач</th>
                </tr>
              </thead>
              <tbody>
                <xsl:for-each select="Discipline">
                  <tr>
                    <td><xsl:value-of select="@Name" /></td>
                    <td><xsl:value-of select="@Semester" /></td>
                    <td><xsl:value-of select="@Credits" /></td>
                    <td><xsl:value-of select="@ExamType" /></td>
                    <td class="grade-high"><xsl:value-of select="@Grade" /></td>
                    <td><xsl:value-of select="@NationalGrade" /></td>
                    <td><xsl:value-of select="@Lecturer" /></td>
                  </tr>
                </xsl:for-each>
              </tbody>
            </table>
          </div>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
